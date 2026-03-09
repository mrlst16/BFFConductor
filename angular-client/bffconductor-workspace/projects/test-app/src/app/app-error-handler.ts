import { Injectable } from '@angular/core';
import { BffErrorHandler, ApiError } from 'bffconductor';

@Injectable()
export class AppErrorHandler implements BffErrorHandler {
  handle(displayMethod: string, errors: ApiError[], headers: Record<string, string>): void {
    console.group(`[BffErrorHandler] displayMethod: "${displayMethod}"`);
    console.table(errors);
    console.log('headers', headers);
    console.groupEnd();
  }
}
