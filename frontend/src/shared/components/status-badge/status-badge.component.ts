import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { TranslationService } from '@core/i18n/translation.service';

/**
 * Reusable status badge component with color coding based on status string
 * and automatic bilingual status label translation.
 *
 * Usage:
 *   <app-status-badge [status]="'Pending'" />
 */
@Component({
  selector: 'app-status-badge',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss'
})
export class StatusBadgeComponent {
  readonly i18n = inject(TranslationService);
  readonly status = input.required<string>();

  getStatusClass(): string {
    const s = this.status().toLowerCase();

    if (['delivered', 'paid', 'approved', 'received', 'active'].includes(s)) {
      return 'success';
    }

    if (['pending', 'draft'].includes(s)) {
      return 'warning';
    }

    if (['cancelled', 'overdue', 'inactive'].includes(s)) {
      return 'error';
    }

    if (['confirmed', 'shipped', 'sent', 'submitted'].includes(s)) {
      return 'info';
    }

    return 'neutral';
  }
}
