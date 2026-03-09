import { HttpErrorResponse, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiResponse } from './models';
import { BFF_ERROR_HANDLER } from './bff-error-handler';

export const BFF_DISPLAY_METHOD_HEADER = 'x-handle-message-as';

export const bffResponseInterceptor: HttpInterceptorFn = (req, next) => {
  const errorHandler = inject(BFF_ERROR_HANDLER, { optional: true });

  return next(req).pipe(
    map(event => {
      if (!(event instanceof HttpResponse)) {
        return event;
      }

      const body = event.body as ApiResponse<unknown>;
      if (body === null || typeof body !== 'object' || !('success' in body)) {
        return event;
      }

      return event.clone({ body: body.data });
    }),
    catchError(err => {
      if (errorHandler && err instanceof HttpErrorResponse) {
        const body = err.error as ApiResponse<unknown>;
        if (body && typeof body === 'object' && 'success' in body) {
          const displayMethod = err.headers.get(BFF_DISPLAY_METHOD_HEADER) ?? 'toast';
          const headers = headersToRecord(err.headers);
          errorHandler.handle(displayMethod, body.errors ?? [], headers);
        }
      }
      return throwError(() => err);
    }),
  );
};

function headersToRecord(headers: HttpErrorResponse['headers']): Record<string, string> {
  return headers.keys().reduce<Record<string, string>>((acc, key) => {
    acc[key] = headers.get(key) ?? '';
    return acc;
  }, {});
}
