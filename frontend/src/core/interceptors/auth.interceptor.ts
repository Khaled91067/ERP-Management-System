import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { IS_AUTH_RETRY } from '../http/http-context.tokens';

let isRefreshing = false;
let refreshTokenSubject = new BehaviorSubject<string | null>(null);

/**
 * Resets module-scoped refresh state (used for clean unit test isolation).
 */
export function resetAuthInterceptorState(): void {
  isRefreshing = false;
  refreshTokenSubject = new BehaviorSubject<string | null>(null);
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.token();

  let authReq = req;
  if (token && !req.headers.has('Authorization')) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // 403 Forbidden must NEVER trigger token refresh or logout
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // Do not attempt token refresh on auth endpoints (/auth/login, /auth/refresh, /auth/logout)
      // or on requests that have already been retried once.
      const isAuthEndpoint = req.url.includes('/auth/');
      const isAlreadyRetried = req.context.get(IS_AUTH_RETRY);

      if (isAuthEndpoint || isAlreadyRetried) {
        if (!isAuthEndpoint) {
          authService.logout();
        }
        return throwError(() => error);
      }

      const refreshToken = authService.refreshToken();
      if (!refreshToken) {
        authService.logout();
        return throwError(() => error);
      }

      if (!isRefreshing) {
        isRefreshing = true;
        refreshTokenSubject.next(null);

        return authService.refreshAccessToken().pipe(
          switchMap(res => {
            isRefreshing = false;
            const newToken = res?.accessToken || res?.token || '';
            if (newToken) {
              refreshTokenSubject.next(newToken);
              const retryReq = req.clone({
                context: req.context.set(IS_AUTH_RETRY, true),
                setHeaders: {
                  Authorization: `Bearer ${newToken}`
                }
              });
              return next(retryReq);
            }

            refreshTokenSubject.error(error);
            refreshTokenSubject = new BehaviorSubject<string | null>(null);
            authService.logout();
            return throwError(() => error);
          }),
          catchError(refreshErr => {
            isRefreshing = false;
            refreshTokenSubject.error(refreshErr);
            refreshTokenSubject = new BehaviorSubject<string | null>(null);
            authService.logout();
            return throwError(() => refreshErr);
          })
        );
      }

      // Concurrent 401 request while refresh is already in progress:
      // wait for the in-flight refresh to emit the new token and retry.
      return refreshTokenSubject.pipe(
        filter((newToken): newToken is string => newToken !== null),
        take(1),
        switchMap(newToken => {
          const retryReq = req.clone({
            context: req.context.set(IS_AUTH_RETRY, true),
            setHeaders: {
              Authorization: `Bearer ${newToken}`
            }
          });
          return next(retryReq);
        })
      );
    })
  );
};
