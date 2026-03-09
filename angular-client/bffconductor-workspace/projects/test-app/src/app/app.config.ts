import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { BFF_ERROR_HANDLER, bffResponseInterceptor } from 'bffconductor';
import { routes } from './app.routes';
import { AppErrorHandler } from './app-error-handler';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([bffResponseInterceptor])),
    { provide: BFF_ERROR_HANDLER, useClass: AppErrorHandler }
  ]
};
