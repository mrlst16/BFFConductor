import { Injectable } from '@angular/core';
import { BffErrorHandler, ApiError } from 'bffconductor';
import { toast } from 'ngx-sonner';
import { InlineErrorService } from './inline-error.service';

@Injectable()
export class AppErrorHandler implements BffErrorHandler {
  constructor(private inlineErrors: InlineErrorService) {}

  handle(displayMethod: string, errors: ApiError[], _headers: Record<string, string>): void {
    const message = errors[0]?.message ?? 'An error occurred.';

    switch (displayMethod) {
      case 'toast':
        toast.error(message, { position: 'top-right' });
        break;
      case 'snackbar':
        toast.error(message, { position: 'bottom-center' });
        break;
      case 'inline':
        this.inlineErrors.set(errors);
        break;
      case 'silent':
        break;
    }
  }
}
