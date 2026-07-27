import {
  ChangeDetectionStrategy,
  Component,
  input,
  output,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

export interface Breadcrumb {
  label: string;
  link?: string;
}

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
  imports: [RouterLink, MatButtonModule, MatIconModule, MatTooltipModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="page-header">
      @if (breadcrumbs().length > 0) {
        <nav class="page-header__breadcrumbs" aria-label="Breadcrumb">
          @for (crumb of breadcrumbs(); track crumb.label; let last = $last) {
            @if (crumb.link && !last) {
              <a [routerLink]="crumb.link" class="page-header__crumb page-header__crumb--link">
                {{ crumb.label }}
              </a>
            } @else {
              <span class="page-header__crumb" [class.page-header__crumb--active]="last">
                {{ crumb.label }}
              </span>
            }
            @if (!last) {
              <span class="page-header__separator" aria-hidden="true">/</span>
            }
          }
        </nav>
      }

      <div class="page-header__main">
        <div class="page-header__text">
          <h1 class="page-header__title">{{ title() }}</h1>
          @if (description()) {
            <p class="page-header__description">{{ description() }}</p>
          }
        </div>

        @if (actionLabel()) {
          <button
            mat-flat-button
            color="primary"
            class="page-header__action"
            (click)="action.emit()"
          >
            @if (actionIcon()) {
              <mat-icon>{{ actionIcon() }}</mat-icon>
            }
            {{ actionLabel() }}
          </button>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin-bottom: 32px;
    }

    .page-header__main {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: 16px;
    }

    @media (max-width: 600px) {
      .page-header__main {
        flex-direction: column;
        align-items: stretch;
      }
    }

    .page-header__text {
      display: flex;
      flex-direction: column;
    }

    .page-header__breadcrumbs {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .page-header__crumb {
      font-size: 0.75rem;
      color: var(--text-tertiary);
    }

    .page-header__crumb--link {
      text-decoration: none;
      
      &:hover { 
        text-decoration: underline;
        color: var(--text-secondary);
      }
    }

    .page-header__crumb--active {
      color: var(--text-secondary);
      font-weight: 500;
    }

    .page-header__separator {
      color: var(--text-tertiary);
      font-size: 0.75rem;
    }

    .page-header__title {
      margin: 0;
      font-size: 1.75rem;
      font-weight: 700;
      color: var(--text-primary);
      line-height: 1.2;
      letter-spacing: -0.02em;
    }

    .page-header__description {
      margin: 6px 0 0 0;
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .page-header__action {
      white-space: nowrap;
      flex-shrink: 0;
    }
  `],
})
export class PageHeaderComponent {
  readonly title = input.required<string>();
  readonly description = input<string>();
  readonly breadcrumbs = input<Breadcrumb[]>([]);
  readonly actionLabel = input<string>('');
  readonly actionIcon = input<string>('');
  readonly action = output<void>();
}
