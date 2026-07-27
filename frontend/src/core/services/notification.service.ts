import { inject, Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

type NotificationType = 'success' | 'error' | 'warning' | 'info';

const DEFAULT_DURATION = 4000;

const PANEL_CLASS: Record<NotificationType, string> = {
  success: 'snack-success',
  error: 'snack-error',
  warning: 'snack-warning',
  info: 'snack-info',
};

/**
 * Thin wrapper around MatSnackBar for consistent toast notifications.
 * Exposes simple success/error/warning/info helpers used across all features.
 */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);

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

  private show(
    message: string,
    type: NotificationType,
    duration: number
  ): void {
    const config: MatSnackBarConfig = {
      duration,
      horizontalPosition: 'right',
      verticalPosition: 'top',
      panelClass: ['erp-snack', PANEL_CLASS[type]],
    };
    this.snackBar.open(message, '✕', config);
  }
}
