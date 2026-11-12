import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  output,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslationService } from '@core/i18n/translation.service';
import { Breadcrumb } from '../../models/breadcrumb.model';

/**
 * Page-level header with title, optional breadcrumbs, and a primary action button.
 */
@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './page-header.component.html',
  styleUrl: './page-header.component.scss'
})
export class PageHeaderComponent {
  readonly i18n = inject(TranslationService);
  readonly title = input.required<string>();
  readonly description = input<string>();
  readonly breadcrumbs = input<Breadcrumb[]>([]);
  readonly actionLabel = input<string>('');
  readonly actionIcon = input<string>('');
  readonly action = output<void>();

  translateCrumb(label: string): string {
    // Check if label is already a translation key or known navigation/common label
    if (label.includes('.')) {
      return this.i18n.translate(label);
    }
    const groupKey = `sidebar.groups.${label}`;
    const groupVal = this.i18n.translate(groupKey);
    if (groupVal !== groupKey) {
      return groupVal;
    }
    const itemKey = `sidebar.items.${label}`;
    const itemVal = this.i18n.translate(itemKey);
    if (itemVal !== itemKey) {
      return itemVal;
    }
    if (label === 'New') {
      return this.i18n.translate('common.new');
    }
    if (label === 'Edit') {
      return this.i18n.translate('common.edit');
    }
    return label;
  }
}
