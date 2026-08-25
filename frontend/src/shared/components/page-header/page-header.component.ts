import {
  ChangeDetectionStrategy,
  Component,
  input,
  output,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Breadcrumb } from '../../models/breadcrumb.model';

/**
 * Page-level header with title, optional breadcrumbs, and a primary action button.
 *
 * Usage:
 *   <app-page-header
 *     title="Users"
 *     [breadcrumbs]="[{ label: 'Settings' }, { label: 'Users' }]"
 *     actionLabel="New User"
 *     actionIcon="add"
 *     (action)="openForm()"
 *   />
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
  readonly title = input.required<string>();
  readonly description = input<string>();
  readonly breadcrumbs = input<Breadcrumb[]>([]);
  readonly actionLabel = input<string>('');
  readonly actionIcon = input<string>('');
  readonly action = output<void>();
}
