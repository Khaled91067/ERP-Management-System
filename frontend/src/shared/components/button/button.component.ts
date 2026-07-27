import { Component, input, output } from '@angular/core';

export type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'danger' | 'success';
export type ButtonSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-button',
  standalone: true,
  template: `
    <button
      [type]="type()"
      [disabled]="disabled() || loading()"
      [class]="'btn btn-' + variant() + ' btn-' + size()"
      (click)="onClick($event)"
    >
      @if (loading()) {
        <span class="spinner"></span>
      }
      <ng-content />
    </button>
  `,
  styles: [`
    .btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      font-weight: 500;
      border-radius: 0.375rem;
      border: 1px solid transparent;
      cursor: pointer;
      transition: background-color 0.2s, border-color 0.2s, box-shadow 0.2s;
    }
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .btn-sm { padding: 0.25rem 0.625rem; font-size: 0.875rem; }
    .btn-md { padding: 0.5rem 1rem; font-size: 1rem; }
    .btn-lg { padding: 0.75rem 1.5rem; font-size: 1.125rem; }

    .btn-primary { background-color: var(--primary-color); color: #ffffff; }
    .btn-primary:hover:not(:disabled) { background-color: var(--primary-hover); }

    .btn-secondary { background-color: var(--text-secondary); color: #ffffff; }
    .btn-secondary:hover:not(:disabled) { opacity: 0.9; }

    .btn-outline { background-color: transparent; border-color: var(--border-color); color: var(--text-primary); }
    .btn-outline:hover:not(:disabled) { background-color: var(--hover-bg); }

    .btn-danger { background-color: var(--error-color); color: #ffffff; }
    .btn-danger:hover:not(:disabled) { filter: brightness(0.9); }

    .btn-success { background-color: var(--success-color); color: #ffffff; }
    .btn-success:hover:not(:disabled) { filter: brightness(0.9); }

    .spinner {
      width: 1rem;
      height: 1rem;
      border: 2px solid currentColor;
      border-right-color: transparent;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class ButtonComponent {
  readonly variant = input<ButtonVariant>('primary');
  readonly size = input<ButtonSize>('md');
  readonly type = input<'button' | 'submit' | 'reset'>('button');
  readonly disabled = input<boolean>(false);
  readonly loading = input<boolean>(false);

  readonly btnClick = output<MouseEvent>();

  protected onClick(event: MouseEvent): void {
    if (!this.disabled() && !this.loading()) {
      this.btnClick.emit(event);
    }
  }
}
