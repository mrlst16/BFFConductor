import { Component, signal } from '@angular/core';
import { InlineErrorService } from '../inline-error.service';
import { TestService } from '../test.service';

// Controller overrides: 1001 → toast,    1002 → silent
// Action overrides:     1001 → snackbar  (further override), 1002 → silent (inherited)
const SUCCESS_SCENARIO = { label: 'Success', displayMode: null, errorCode: null, errorMessage: null };
const ERROR_SCENARIOS = [
  { label: 'Validation', displayMode: 'snackbar', errorCode: '1001', errorMessage: 'Validation failed.' },
  { label: 'Not Found',  displayMode: 'silent',   errorCode: '1002', errorMessage: 'Resource not found.' },
];

@Component({
  selector: 'app-action-test',
  imports: [],
  templateUrl: './action-test.html',
})
export class ActionTest {
  protected readonly successScenario = SUCCESS_SCENARIO;
  protected readonly errorScenarios = ERROR_SCENARIOS;
  protected result = signal<'idle' | 'success'>('idle');
  protected readonly inlineErrors;

  constructor(private testService: TestService, inlineErrorService: InlineErrorService) {
    this.inlineErrors = inlineErrorService.errors;
  }

  send(errorCode: string | null, errorMessage: string | null): void {
    this.result.set('idle');
    this.testService.postActionLevel({ errorCode, errorMessage }).subscribe({
      next: () => this.result.set('success'),
      error: () => {},
    });
  }
}
