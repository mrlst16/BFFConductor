import { InjectionToken } from '@angular/core';
import { ApiError } from './models';

export interface BffErrorHandler {
  handle(displayMethod: string, errors: ApiError[], headers: Record<string, string>): void;
}

export const BFF_ERROR_HANDLER = new InjectionToken<BffErrorHandler>('BffErrorHandler');
