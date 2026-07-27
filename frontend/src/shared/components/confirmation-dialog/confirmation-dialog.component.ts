import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  danger?: boolean;
}

/**
 * Reusable confirmation dialog for delete/destructive operations.
 *
 * Usage:
 *   const ref = this.dialog.open(ConfirmationDialogComponent, {
 *     data: { title: 'Delete User', message: 'Are you sure?', danger: true }
 *   });
 *   ref.afterClosed().subscribe(confirmed => { if (confirmed) { ... } });
 */
@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="confirm-dialog">
      <div class="confirm-dialog__header">
        @if (data.danger) {
          <mat-icon class="confirm-dialog__icon confirm-dialog__icon--danger">warning</mat-icon>
        }
        <h2 mat-dialog-title>{{ data.title }}</h2>
      </div>

      <mat-dialog-content>
        <p>{{ data.message }}</p>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button [mat-dialog-close]="false">
          {{ data.cancelText ?? 'Cancel' }}
        </button>
        <button
          mat-flat-button
          [color]="data.danger ? 'warn' : 'primary'"
          [mat-dialog-close]="true"
        >
          {{ data.confirmText ?? 'Confirm' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .confirm-dialog {
      min-width: 360px;
      max-width: 480px;
    }

    .confirm-dialog__header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 20px 24px 0;
    }

    .confirm-dialog__icon--danger {
      color: var(--error-color);
      font-size: 28px;
      width: 28px;
      height: 28px;
    }

    h2[mat-dialog-title] {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
    }

    mat-dialog-content p {
      color: var(--text-secondary);
      margin: 0;
      line-height: 1.6;
    }

    mat-dialog-actions {
      padding: 16px 24px;
      gap: 8px;
    }
  `],
})
export class ConfirmationDialogComponent {
  readonly data = inject<ConfirmDialogData>(MAT_DIALOG_DATA);
  readonly dialogRef = inject(MatDialogRef<ConfirmationDialogComponent>);
}
