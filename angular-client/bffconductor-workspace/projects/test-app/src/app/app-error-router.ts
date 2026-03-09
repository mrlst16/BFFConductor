import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BffErrorRouter, ApiError } from 'bffconductor';
import { toast } from 'ngx-sonner';
import { InlineErrorService } from './inline-error.service';

@Injectable()
export class AppErrorRouter implements BffErrorRouter {
  constructor(private inlineErrors: InlineErrorService, private router: Router) {}

  route(displayMode: string, errors: ApiError[], headers: Record<string, string>): void {
    const message = errors[0]?.message ?? 'An error occurred.';

    switch (displayMode) {
      case 'toast':
        toast.error(message, { position: 'top-right' });
        break;
      case 'snackbar':
        toast.error(message, { position: 'bottom-center' });
        break;
      case 'inline':
        this.inlineErrors.set(errors);
        break;
      case 'redirect': {
        const url = headers['x-redirect-url'] ?? '/error';
        const redirectMessage = headers['x-redirect-message'] ?? message;
        this.router.navigateByUrl(`${url}?message=${encodeURIComponent(redirectMessage)}`);
        break;
      }
      case 'silent':
        break;
    }
  }
}
