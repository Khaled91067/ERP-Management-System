import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ErrorHandlingService } from '@core/errors/error-handling.service';
import { NormalizedApiError } from '@core/models/api-error.model';

export type EnrichedHttpErrorResponse = HttpErrorResponse & {
  normalizedError?: NormalizedApiError;
};

/**
 * Centralized HTTP error interceptor.
 * Normalizes backend error responses (ProblemDetails, ValidationProblemDetails, etc.),
 * maps field validation errors when a form is active, and triggers translated user notifications.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorHandlingService = inject(ErrorHandlingService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const normalized = errorHandlingService.handleHttpError(error, req);
      (error as EnrichedHttpErrorResponse).normalizedError = normalized;
      return throwError(() => error);
    })
  );
};
