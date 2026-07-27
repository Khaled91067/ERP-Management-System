import { Component, input } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  template: `
    <div class="card" [class.card-bordered]="bordered()">
      @if (title()) {
        <div class="card-header">
          <h3 class="card-title">{{ title() }}</h3>
          <ng-content select="[card-header-actions]" />
        </div>
      }
      <div class="card-body">
        <ng-content />
      </div>
      <div class="card-footer">
        <ng-content select="[card-footer]" />
      </div>
    </div>
  `,
  styles: [`
    .card {
      background-color: var(--surface-card, #ffffff);
      border-radius: 0.5rem;
      box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }
    .card-bordered {
      border: 1px solid var(--border-color, #e5e7eb);
    }
    .card-header {
      padding: 1rem 1.25rem;
      border-bottom: 1px solid var(--border-color, #f3f4f6);
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    .card-title {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--text-primary, #111827);
    }
    .card-body {
      padding: 1.25rem;
      flex: 1;
    }
    .card-footer {
      padding: 0.75rem 1.25rem;
      background-color: var(--surface-ground, #f9fafb);
      border-top: 1px solid var(--border-color, #f3f4f6);
    }
    .card-footer:empty {
      display: none;
    }
  `]
})
export class CardComponent {
  readonly title = input<string>('');
  readonly bordered = input<boolean>(true);
}
