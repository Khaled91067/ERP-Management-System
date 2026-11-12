import { inject, Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { TranslationService } from '@core/i18n/translation.service';

type NotificationType = 'success' | 'error' | 'warning' | 'info';

const DEFAULT_DURATION = 4000;
const DEDUP_INTERVAL_MS = 1000;

const PANEL_CLASS: Record<NotificationType, string> = {
  success: 'snack-success',
  error: 'snack-error',
  warning: 'snack-warning',
  info: 'snack-info',
};

/**
 * Thin wrapper around MatSnackBar for consistent, RTL/LTR-aware toast notifications.
 * Exposes simple success/error/warning/info helpers used across all features.
 */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);
  private readonly i18n = inject(TranslationService);

  private lastMessage: string | null = null;
  private lastType: NotificationType | null = null;
  private lastShownAt = 0;

  success(message: string, duration = DEFAULT_DURATION): void {
    this.show(message, 'success', duration);
  }

  error(message: string, duration = DEFAULT_DURATION): void {
    this.show(message, 'error', duration);
  }

  warning(message: string, duration = DEFAULT_DURATION): void {
    this.show(message, 'warning', duration);
  }

  info(message: string, duration = DEFAULT_DURATION): void {
    this.show(message, 'info', duration);
  }

  resetDeduplication(): void {
    this.lastMessage = null;
    this.lastType = null;
    this.lastShownAt = 0;
  }

  private show(
    message: string,
    type: NotificationType,
    duration: number
  ): void {
    const now = Date.now();
    if (
      this.lastMessage === message &&
      this.lastType === type &&
      now - this.lastShownAt < DEDUP_INTERVAL_MS
    ) {
      return;
    }

    this.lastMessage = message;
    this.lastType = type;
    this.lastShownAt = now;

    const config: MatSnackBarConfig = {
      duration,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      direction: this.i18n.dir(),
      panelClass: ['erp-snack', PANEL_CLASS[type]],
    };
    this.snackBar.open(message, '✕', config);
  }
}
