import { HttpContext, HttpContextToken } from '@angular/common/http';

/**
 * When set to true on an HttpRequest context, the global error interceptor
 * will normalize the error but skip displaying a global toast notification.
 */
export const SKIP_GLOBAL_ERROR_NOTIFICATION = new HttpContextToken<boolean>(() => false);

/**
 * Marks a request as an already-retried request after a token refresh
 * to prevent infinite 401 refresh loops.
 */
export const IS_AUTH_RETRY = new HttpContextToken<boolean>(() => false);

export interface ErrorHandlingRequestOptions {
  skipGlobalNotification?: boolean;
}

export function withErrorHandlingOptions(
  options: ErrorHandlingRequestOptions,
  existingContext?: HttpContext
): HttpContext {
  const ctx = existingContext ?? new HttpContext();
  if (options.skipGlobalNotification !== undefined) {
    ctx.set(SKIP_GLOBAL_ERROR_NOTIFICATION, options.skipGlobalNotification);
  }
  return ctx;
}
