import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { BFF_ERROR_ROUTER, bffResponseInterceptor } from 'bffconductor';
import { routes } from './app.routes';
import { AppErrorRouter } from './app-error-router';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([bffResponseInterceptor])),
    { provide: BFF_ERROR_ROUTER, useClass: AppErrorRouter }
  ]
};
