import { Component, signal } from '@angular/core';
import { InlineErrorService } from '../inline-error.service';
import { TestService } from '../test.service';

// Spec defaults:       1001 → inline,   1002 → toast
// Controller overrides: 1001 → toast,    1002 → silent
const SUCCESS_SCENARIO = { label: 'Success', displayMode: null, errorCode: null, errorMessage: null };
const ERROR_SCENARIOS = [
  { label: 'Validation', displayMode: 'toast',  errorCode: '1001', errorMessage: 'Validation failed.' },
  { label: 'Not Found',  displayMode: 'silent', errorCode: '1002', errorMessage: 'Resource not found.' },
];

@Component({
  selector: 'app-controller-test',
  imports: [],
  templateUrl: './controller-test.html',
})
export class ControllerTest {
  protected readonly successScenario = SUCCESS_SCENARIO;
  protected readonly errorScenarios = ERROR_SCENARIOS;
  protected result = signal<'idle' | 'success'>('idle');
  protected readonly inlineErrors;

  constructor(private testService: TestService, inlineErrorService: InlineErrorService) {
    this.inlineErrors = inlineErrorService.errors;
  }

  send(errorCode: string | null, errorMessage: string | null): void {
    this.result.set('idle');
    this.testService.postControllerLevel({ errorCode, errorMessage }).subscribe({
      next: () => this.result.set('success'),
      error: () => {},
    });
  }
}
