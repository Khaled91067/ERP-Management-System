import { Component, input } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  template: `
    <div class="spinner-container" [class.overlay]="overlay()" role="status" aria-live="polite">
      <div class="spinner" [style.width.px]="size()" [style.height.px]="size()"></div>
      @if (label()) {
        <p class="spinner-label">{{ label() }}</p>
      }
    </div>
  `,
  styles: [`
    .spinner-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 1.5rem;
    }
    .spinner-container.overlay {
      position: absolute;
      inset: 0;
      background-color: var(--surface-card);
      opacity: 0.85;
      z-index: 50;
    }
    .spinner {
      border: 3px solid var(--border-color);
      border-top-color: var(--primary-color);
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
    .spinner-label {
      margin-top: 0.75rem;
      font-size: 0.875rem;
      color: var(--text-secondary);
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class LoadingSpinnerComponent {
  readonly size = input<number>(36);
  readonly overlay = input<boolean>(false);
  readonly label = input<string>('');
}
