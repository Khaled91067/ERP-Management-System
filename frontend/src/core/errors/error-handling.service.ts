import { Injectable, inject, signal } from '@angular/core';
import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { SKIP_GLOBAL_ERROR_NOTIFICATION } from '@core/http/http-context.tokens';
import {
  ApiErrorCategory,
  NormalizedApiError,
  ProblemDetailsPayload
} from '@core/models/api-error.model';
import { FormValidationService } from './form-validation.service';

const DEDUP_WINDOW_MS = 1500;

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {
  private readonly i18n = inject(TranslationService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);

  readonly lastError = signal<NormalizedApiError | null>(null);

  private lastNotifiedKey: string | null = null;
  private lastNotifiedTime = 0;

  /**
   * Central entry point for HTTP error processing from the HTTP interceptor.
   */
  handleHttpError(error: HttpErrorResponse, req?: HttpRequest<unknown>): NormalizedApiError {
    const normalized = this.normalizeHttpError(error, req?.url);

    // If a FormGroup is actively submitting and we received field-level validation errors,
    // map them directly to the form controls first.
    const activeForm = this.formValidation.getActiveForm();
    if (activeForm && Object.keys(normalized.fieldErrors).length > 0) {
      const result = this.formValidation.applyServerErrors(activeForm, normalized);
      if (result.handledByFields) {
        normalized.handledByForm = true;
      }
    } else if (activeForm) {
      this.formValidation.clearActiveForm(activeForm);
    }

    this.lastError.set(normalized);

    const skipGlobalNotification = req?.context.get(SKIP_GLOBAL_ERROR_NOTIFICATION) ?? false;
    if (!skipGlobalNotification && !normalized.handledByForm) {
      this.notifyUser(normalized);
    }

    return normalized;
  }

  /**
   * Normalizes any HttpErrorResponse into a strongly-typed, predictable NormalizedApiError
   * compatible with RFC 7807 ProblemDetails, ValidationProblemDetails, raw string errors,
   * and empty middleware bodies.
   */
  normalizeHttpError(error: HttpErrorResponse, requestUrl?: string): NormalizedApiError {
    const status = error?.status ?? 0;
    const payload = this.parsePayload(error?.error);
    const fieldErrors = this.extractFieldErrors(payload?.errors);
    const category = this.resolveCategory(status, fieldErrors);

    const correlationId =
      this.sanitizeIdentifier(payload?.correlationId) ??
      this.sanitizeIdentifier(payload?.traceId) ??
      error?.headers?.get('X-Correlation-Id') ??
      error?.headers?.get('x-correlation-id') ??
      null;

    const rawTitle = typeof payload?.title === 'string' ? payload.title.trim() : null;
    const rawDetail = this.extractSafeDetail(error?.error, payload);
    const rawType = typeof payload?.type === 'string' ? payload.type.trim() : null;

    const translationKey = this.resolveTranslationKey(
      status,
      category,
      rawTitle,
      rawDetail,
      rawType,
      fieldErrors,
      requestUrl ?? error?.url ?? undefined
    );

    const userMessage = this.i18n.translate(translationKey);

    return {
      status,
      category,
      type: rawType,
      title: rawTitle,
      detail: rawDetail,
      translationKey,
      userMessage,
      fieldErrors,
      correlationId,
      timestamp: new Date().toISOString(),
      handledByForm: false
    };
  }

  /**
   * Resets deduplication state (useful in unit tests).
   */
  resetNotificationDeduplication(): void {
    this.lastNotifiedKey = null;
    this.lastNotifiedTime = 0;
  }

  private notifyUser(normalized: NormalizedApiError): void {
    const now = Date.now();
    const dedupSignature = `${normalized.status}:${normalized.translationKey}`;

    if (this.lastNotifiedKey === dedupSignature && now - this.lastNotifiedTime < DEDUP_WINDOW_MS) {
      return;
    }

    this.lastNotifiedKey = dedupSignature;
    this.lastNotifiedTime = now;

    // Re-translate at notification time so current language is always respected
    const message = this.i18n.translate(normalized.translationKey, normalized.translationParams);

    if (normalized.category === 'rateLimit') {
      this.notification.warning(message);
    } else {
      this.notification.error(message);
    }
  }

  private parsePayload(rawError: unknown): ProblemDetailsPayload | null {
    if (!rawError) {
      return null;
    }

    if (typeof rawError === 'object' && !(rawError instanceof Blob) && !(rawError instanceof ProgressEvent)) {
      return rawError as ProblemDetailsPayload;
    }

    if (typeof rawError === 'string') {
      const trimmed = rawError.trim();
      if (trimmed.startsWith('{') && trimmed.endsWith('}')) {
        try {
          const parsed = JSON.parse(trimmed);
          if (parsed && typeof parsed === 'object') {
            return parsed as ProblemDetailsPayload;
          }
        } catch {
          // Not JSON; handled as plain string in extractSafeDetail
        }
      }
    }

    return null;
  }

  private extractFieldErrors(
    rawErrors: Record<string, string[] | string> | null | undefined
  ): Record<string, string[]> {
    if (!rawErrors || typeof rawErrors !== 'object') {
      return {};
    }

    const normalized: Record<string, string[]> = {};
    for (const [field, messages] of Object.entries(rawErrors)) {
      if (Array.isArray(messages)) {
        const valid = messages
          .filter((m): m is string => typeof m === 'string' && m.trim().length > 0)
          .map(m => m.trim());
        if (valid.length > 0) {
          normalized[field] = valid;
        }
      } else if (typeof messages === 'string' && messages.trim().length > 0) {
        normalized[field] = [messages.trim()];
      }
    }

    return normalized;
  }

  private resolveCategory(status: number, fieldErrors: Record<string, string[]>): ApiErrorCategory {
    if (status === 0) {
      return 'network';
    }

    const hasFieldErrors = Object.keys(fieldErrors).length > 0;

    switch (status) {
      case 400:
        return hasFieldErrors ? 'validation' : 'badRequest';
      case 401:
        return 'unauthorized';
      case 403:
        return 'forbidden';
      case 404:
        return 'notFound';
      case 409:
        return 'conflict';
      case 422:
        return hasFieldErrors ? 'validation' : 'unprocessable';
      case 429:
        return 'rateLimit';
      case 502:
      case 503:
      case 504:
        return 'serviceUnavailable';
      default:
        return status >= 500 ? 'serverError' : 'badRequest';
    }
  }

  /**
   * Maps stable status codes, error categories, known ProblemDetails titles, and safe domain
   * rule patterns to frontend translation keys. Never uses backend detail/title as translation keys.
   */
  private resolveTranslationKey(
    status: number,
    category: ApiErrorCategory,
    title: string | null,
    detail: string | null,
    type: string | null,
    fieldErrors: Record<string, string[]>,
    url?: string
  ): string {
    if (status === 0 || category === 'network') {
      return 'errors.networkError';
    }

    const lowerTitle = (title ?? '').toLowerCase();
    const lowerDetail = (detail ?? '').toLowerCase();
    const lowerType = (type ?? '').toLowerCase();
    const hasFieldErrors = Object.keys(fieldErrors).length > 0;

    switch (status) {
      case 400: {
        if (hasFieldErrors || lowerTitle === 'validation error' || lowerTitle.includes('validation errors')) {
          return 'errors.validation';
        }
        if (
          lowerDetail.includes('mismatch between') ||
          lowerDetail.includes('mismatch in request') ||
          (lowerDetail.includes('id') && lowerDetail.includes('does not match'))
        ) {
          return 'errors.idMismatch';
        }
        if (lowerDetail.includes('invalid payment method')) {
          return 'errors.invalidPaymentMethod';
        }
        if (lowerDetail.includes('insufficient stock') || lowerDetail.includes('not enough stock')) {
          return 'errors.insufficientStock';
        }
        if (lowerDetail.includes('already cancelled') || lowerDetail.includes('already canceled')) {
          return 'errors.orderAlreadyCancelled';
        }
        if (lowerDetail.includes('already paid')) {
          return 'errors.invoiceAlreadyPaid';
        }
        if (
          lowerDetail.includes('status') &&
          (lowerDetail.includes('cannot') || lowerDetail.includes('only ') || lowerDetail.includes('invalid'))
        ) {
          return 'errors.invalidStatusTransition';
        }
        return 'errors.badRequest';
      }

      case 401: {
        const isLoginRequest = url ? url.toLowerCase().includes('/auth/login') : false;
        if (isLoginRequest || lowerDetail.includes('invalid email or password')) {
          return 'errors.invalidCredentials';
        }
        return 'errors.sessionExpired';
      }

      case 403:
        return 'errors.forbidden';

      case 404:
        return 'errors.notFound';

      case 409: {
        if (lowerTitle === 'concurrency conflict' || lowerDetail.includes('modified by another user')) {
          return 'errors.concurrencyConflict';
        }
        if (lowerDetail.includes('email') && lowerDetail.includes('already exists')) {
          return 'errors.duplicateEmail';
        }
        if (lowerDetail.includes('sku') && lowerDetail.includes('already exists')) {
          return 'errors.duplicateSku';
        }
        if (lowerDetail.includes('name') && lowerDetail.includes('already exists')) {
          return 'errors.duplicateName';
        }
        if (lowerDetail.includes('unique identifier') || lowerDetail.includes('already exists')) {
          return 'errors.duplicateRecord';
        }
        if (
          lowerDetail.includes('referenced') ||
          lowerDetail.includes('in use') ||
          lowerDetail.includes('cannot delete')
        ) {
          return 'errors.cannotDeleteInUse';
        }
        return 'errors.conflict';
      }

      case 422: {
        if (hasFieldErrors || lowerType.includes('validation')) {
          return 'errors.validation';
        }
        return 'errors.unprocessable';
      }

      case 429:
        return 'errors.rateLimit';

      case 502:
      case 503:
      case 504:
        return 'errors.serviceUnavailable';

      case 500:
      default:
        return status >= 500 ? 'errors.serverError' : 'errors.unknownError';
    }
  }

  /**
   * Extracts a sanitized detail string for internal diagnostic inspection while stripping
   * any stack traces, SQL statements, or internal .NET exception class names.
   */
  private extractSafeDetail(rawError: unknown, payload: ProblemDetailsPayload | null): string | null {
    let candidate: string | null = null;

    if (typeof payload?.detail === 'string' && payload.detail.trim().length > 0) {
      candidate = payload.detail.trim();
    } else if (typeof rawError === 'string' && rawError.trim().length > 0) {
      const trimmed = rawError.trim();
      if (!trimmed.startsWith('<') && !trimmed.startsWith('{')) {
        candidate = trimmed;
      }
    }

    if (!candidate) {
      return null;
    }

    if (this.containsSensitiveTechnicalDetails(candidate)) {
      return null;
    }

    return candidate;
  }

  private containsSensitiveTechnicalDetails(text: string): boolean {
    const sensitivePatterns = [
      /\bat\s+[\w.]+\s*\(/i, // Stack trace frame
      /System\.[A-Za-z0-9_.]+Exception/i,
      /Microsoft\.EntityFrameworkCore/i,
      /Microsoft\.Data\.SqlClient/i,
      /SqlException/i,
      /InnerException/i,
      /SELECT\s+.*\s+FROM\s+/i,
      /INSERT\s+INTO\s+/i,
      /UPDATE\s+\w+\s+SET\s+/i,
      /DELETE\s+FROM\s+/i,
      /StackTrace/i
    ];

    return sensitivePatterns.some(pattern => pattern.test(text));
  }

  private sanitizeIdentifier(value: unknown): string | null {
    if (typeof value === 'string' && value.trim().length > 0) {
      return value.trim();
    }
    return null;
  }
}
