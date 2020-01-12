import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Injectable, isDevMode } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import * as Sentry from '@sentry/browser';
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        // if (!isDevMode()) {
        // const eventId = Sentry.captureException(error.originalError || error);
        // }
        if (error instanceof HttpErrorResponse) {
          const appError = error.headers.get('Application-Error');
          if (appError) {
            console.log(appError);
            return throwError(appError);
          }

          const serverError = error.error;
          let modelError = null;
          if (serverError.status === 401) {
            return throwError({
              status: serverError.status,
              errorText: serverError.title
            });
          }
          if (serverError && serverError.status === 400) {
            modelError = serverError.errors;
          }
          return throwError(modelError || serverError || 'Server Error');
        }
      })
    );
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
