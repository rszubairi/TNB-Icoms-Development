import { ErrorHandler, Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable()
export class GlobalErrorHandlerService implements ErrorHandler {
  private http = inject(HttpClient);

  handleError(error: unknown): void {
    console.error(error);

    const err = error as { message?: string; stack?: string } | undefined;
    const message = err?.message ?? String(error);
    const stackTrace = err?.stack ?? null;

    this.http
      .post(`${environment.apiUrl}/errors/client`, {
        message,
        stackTrace,
        url: window.location.href,
        severity: 'Error'
      })
      .subscribe({ error: () => {} }); // never let reporting-the-error itself throw a second error
  }
}
