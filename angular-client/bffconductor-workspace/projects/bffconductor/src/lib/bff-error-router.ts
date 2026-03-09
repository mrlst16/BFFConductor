import { InjectionToken } from '@angular/core';
import { ApiError } from './models';

export interface BffErrorRouter {
  route(displayMode: string, errors: ApiError[], headers: Record<string, string>): void;
}

export const BFF_ERROR_ROUTER = new InjectionToken<BffErrorRouter>('BffErrorRouter');
