import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, ErrorHandler, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { authInterceptor } from './core/interceptors/auth.interceptor';
import { GlobalErrorHandlerService } from './core/services/global-error-handler.service';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    { provide: ErrorHandler, useClass: GlobalErrorHandlerService }
  ]
};
