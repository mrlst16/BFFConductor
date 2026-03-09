import { Component, signal } from '@angular/core';
import { TestService } from './test.service';

type ResultState =
  | { kind: 'idle' }
  | { kind: 'success' }
  | { kind: 'error'; status: number };

const SUCCESS_SCENARIO = { label: 'Success', errorCode: null, errorMessage: null };

const ERROR_SCENARIOS = [
  { label: 'Validation',   errorCode: '1001', errorMessage: 'Validation failed.' },
  { label: 'Not Found',    errorCode: '1002', errorMessage: 'Resource not found.' },
  { label: 'Unauthorized', errorCode: '1003', errorMessage: 'Unauthorized.' },
  { label: 'Unknown',      errorCode: '0',    errorMessage: 'Something went wrong.' },
];

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly successScenario = SUCCESS_SCENARIO;
  protected readonly errorScenarios = ERROR_SCENARIOS;
  protected result = signal<ResultState>({ kind: 'idle' });

  constructor(private testService: TestService) {}

  send(errorCode: string | null, errorMessage: string | null): void {
    this.result.set({ kind: 'idle' });
    this.testService.post({ errorCode, errorMessage }).subscribe({
      next: () => this.result.set({ kind: 'success' }),
      error: err => this.result.set({ kind: 'error', status: err.status }),
    });
  }
}
