import { Component, signal } from '@angular/core';
import { NgxSonnerToaster } from 'ngx-sonner';
import { InlineErrorService } from './inline-error.service';
import { TestService } from './test.service';

type ResultState =
  | { kind: 'idle' }
  | { kind: 'success' }
  | { kind: 'error'; status: number };

const SUCCESS_SCENARIO = { label: 'Success', displayMethod: null, errorCode: null, errorMessage: null };

const ERROR_SCENARIOS = [
  { label: 'Validation',   displayMethod: 'inline',   errorCode: '1001', errorMessage: 'Validation failed.' },
  { label: 'Not Found',    displayMethod: 'toast',    errorCode: '1002', errorMessage: 'Resource not found.' },
  { label: 'Unauthorized', displayMethod: 'snackbar', errorCode: '1003', errorMessage: 'Unauthorized.' },
  { label: 'Unknown',      displayMethod: 'silent',   errorCode: '0',    errorMessage: 'Something went wrong.' },
];

@Component({
  selector: 'app-root',
  imports: [NgxSonnerToaster],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly successScenario = SUCCESS_SCENARIO;
  protected readonly errorScenarios = ERROR_SCENARIOS;
  protected result = signal<ResultState>({ kind: 'idle' });
  protected readonly inlineErrors;

  constructor(private testService: TestService, inlineErrorService: InlineErrorService) {
    this.inlineErrors = inlineErrorService.errors;
  }

  send(errorCode: string | null, errorMessage: string | null): void {
    this.result.set({ kind: 'idle' });
    this.testService.post({ errorCode, errorMessage }).subscribe({
      next: () => this.result.set({ kind: 'success' }),
      error: err => this.result.set({ kind: 'error', status: err.status }),
    });
  }
}
