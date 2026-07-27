import { Component, inject } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `
    <footer class="footer">
      <div class="footer-content">
        <span class="copyright">&copy; {{ currentYear }} {{ config.appName }} v1.0</span>
        <span class="status-indicator">
          <span class="status-dot"></span> Connected
        </span>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      height: 36px;
      background-color: var(--footer-bg);
      border-top: 1px solid var(--border-divider);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0 24px;
      color: var(--text-tertiary);
    }
    .footer-content {
      width: 100%;
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 0.6875rem;
    }
    .status-indicator {
      display: flex;
      align-items: center;
      gap: 6px;
    }
    .status-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background-color: var(--status-success-text);
    }
  `]
})
export class FooterComponent {
  readonly config = inject(APP_CONFIG);
  readonly currentYear = new Date().getFullYear();
}
