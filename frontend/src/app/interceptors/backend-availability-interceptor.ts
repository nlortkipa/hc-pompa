import { HttpErrorResponse, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, tap, throwError, timeout, TimeoutError } from 'rxjs';
import { BackendAvailabilityService } from '../services/backend-availability.service';
import { ConfigService } from '../services/config.service';

const REQUEST_TIMEOUT_MS = 30000;
const UNAVAILABLE_STATUS_CODES = [502, 503, 504];

export const backendAvailabilityInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.endsWith('/config.json')) {
    return next(req);
  }

  const availability = inject(BackendAvailabilityService);
  const configService = inject(ConfigService);

  if (!req.url.startsWith(configService.backendUrl)) {
    return next(req);
  }

  let request$ = next(req);
  const isLongRunningRequest = req.body instanceof FormData || req.responseType === 'blob';

  if (!isLongRunningRequest) {
    request$ = request$.pipe(timeout({ first: REQUEST_TIMEOUT_MS }));
  }

  return request$.pipe(
    tap(event => {
      if (event instanceof HttpResponse) {
        availability.markAvailable();
      }
    }),
    catchError(error => {
      if (isBackendUnavailableError(error)) {
        availability.markUnavailable();
      } else {
        availability.markAvailable();
      }

      return throwError(() => error);
    })
  );
};

function isBackendUnavailableError(error: unknown): boolean {
  if (error instanceof TimeoutError) return true;

  return error instanceof HttpErrorResponse &&
    (error.status === 0 || UNAVAILABLE_STATUS_CODES.includes(error.status));
}
