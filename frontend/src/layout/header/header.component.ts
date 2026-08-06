import { Component, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '@core/auth/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatTooltipModule],
  template: `
    <header class="header">
      <div class="header-left">
        <button 
          type="button" 
          mat-icon-button 
          class="mobile-menu-btn"
          (click)="toggleSidebar.emit()"
          aria-label="Toggle Sidebar">
          <mat-icon>menu</mat-icon>
        </button>
        <div class="search-box">
          <mat-icon class="search-icon">search</mat-icon>
          <input type="text" class="search-input" placeholder="Search..." readonly />
          <kbd class="search-kbd">Ctrl K</kbd>
        </div>
      </div>

      <div class="header-right">
        <button 
          type="button"
          mat-icon-button 
          class="header-icon-btn"
          matTooltip="Notifications">
          <mat-icon>notifications_none</mat-icon>
        </button>

        @if (authService.currentUser(); as user) {
          <div class="user-profile">
            <div class="user-avatar">
              {{ getUserInitials(user) }}
            </div>
            <div class="user-info">
              <span class="user-name">{{ user.firstName }} {{ user.lastName }}</span>
              <span class="user-role-badge">{{ user.role || 'User' }}</span>
            </div>
            <button 
              type="button"
              mat-icon-button 
              class="logout-icon-btn" 
              (click)="authService.logout()" 
              matTooltip="Logout"
              aria-label="Logout">
              <mat-icon>logout</mat-icon>
            </button>
          </div>
        }
      </div>
    </header>
  `,
  styles: [`
    .header {
      height: 60px;
      background-color: var(--header-bg);
      border-bottom: 1px solid var(--border-default);
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 1.5rem;
      color: var(--text-primary);
      box-sizing: border-box;
    }
    .header-left, .header-right {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .mobile-menu-btn {
      display: none;
      color: var(--text-secondary);
    }

    /* Enterprise Search Input */
    .search-box {
      display: flex;
      align-items: center;
      gap: 8px;
      width: 320px;
      height: 36px;
      padding: 0 12px;
      background-color: var(--bg-app);
      border: 1px solid var(--border-default);
      border-radius: 8px;
      cursor: pointer;
      transition: border-color 0.15s ease, box-shadow 0.15s ease;

      &:hover {
        border-color: var(--border-strong);
      }

      .search-icon {
        font-size: 18px;
        width: 18px;
        height: 18px;
        color: var(--text-tertiary);
      }

      .search-input {
        flex: 1;
        border: none;
        background: transparent;
        outline: none;
        font-size: 0.8125rem;
        color: var(--text-primary);
        cursor: pointer;

        &::placeholder {
          color: var(--text-placeholder);
        }
      }

      .search-kbd {
        font-family: inherit;
        font-size: 11px;
        font-weight: 500;
        color: var(--text-tertiary);
        background-color: var(--surface-card);
        border: 1px solid var(--border-default);
        border-radius: 4px;
        padding: 2px 6px;
        line-height: 1;
      }
    }

    .header-icon-btn {
      color: var(--text-secondary);

      &:hover {
        color: var(--text-primary);
        background-color: var(--btn-hover-bg);
      }
    }

    .user-profile {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding-left: 0.5rem;
      border-left: 1px solid var(--border-divider);
    }

    .user-avatar {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      background-color: var(--primary-light);
      color: var(--primary-base);
      font-size: 0.8125rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      justify-content: center;
      user-select: none;
    }

    .user-info {
      display: flex;
      flex-direction: column;
      gap: 1px;
    }
    .user-name {
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--text-primary);
      line-height: 1.2;
    }
    .user-role-badge {
      font-size: 0.625rem;
      color: var(--text-tertiary);
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      line-height: 1.2;
    }

    .logout-icon-btn {
      color: var(--text-tertiary);

      &:hover {
        color: var(--status-error-text);
        background-color: var(--status-error-bg);
      }
    }

    @media (max-width: 768px) {
      .header {
        padding: 0 0.5rem;
      }
      .header-left, .header-right {
        gap: 0.25rem;
      }
      .mobile-menu-btn {
        display: flex;
      }
      .search-box {
        width: 140px;
        padding: 0 8px;
        .search-kbd {
          display: none;
        }
      }
    }

    @media (max-width: 600px) {
      .user-info {
        display: none;
      }
      .user-profile {
        padding-left: 0.25rem;
      }
      .search-box {
        width: 100px;
        border: none;
        background: transparent;
      }
      .search-input {
        display: none;
      }
    }
  `]
})
export class HeaderComponent {
  readonly authService = inject(AuthService);

  readonly collapsed = input<boolean>(false);
  readonly toggleSidebar = output<void>();

  getUserInitials(user: { firstName?: string; lastName?: string }): string {
    const first = user.firstName ? user.firstName.charAt(0).toUpperCase() : '';
    const last = user.lastName ? user.lastName.charAt(0).toUpperCase() : '';
    return first + last || 'U';
  }
}
