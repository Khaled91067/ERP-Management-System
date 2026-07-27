import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { filter } from 'rxjs/operators';

export interface NavGroup {
  name: string;
  icon?: string;
  items: NavItem[];
  hideHeader?: boolean;
}

export interface NavItem {
  label: string;
  route: string;
  icon: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, MatIconModule, MatTooltipModule],
  template: `
    <aside class="sidebar-inner">
      <div class="brand" [class.collapsed-brand]="collapsed()">
        <div class="brand-left">
          <mat-icon class="brand-icon">business</mat-icon>
          @if (!collapsed()) {
            <span class="brand-text">ERP Admin</span>
          }
        </div>
        <button 
          type="button" 
          class="collapse-toggle-btn"
          (click)="toggleCollapse.emit()" 
          [matTooltip]="collapsed() ? 'Expand Sidebar' : 'Collapse Sidebar'"
          [attr.aria-label]="collapsed() ? 'Expand Sidebar' : 'Collapse Sidebar'">
          <mat-icon>{{ collapsed() ? 'menu' : 'menu_open' }}</mat-icon>
        </button>
      </div>

      <nav class="nav-menu">
        @for (group of navGroups; track group.name) {
          <div class="nav-group" [class.is-expanded]="isGroupExpanded(group.name)">
            @if (!collapsed() && !group.hideHeader) {
              <button 
                type="button" 
                class="nav-group-header" 
                (click)="toggleGroup(group.name)"
                [class.active-group]="isGroupActive(group)">
                <div class="group-header-left">
                  @if (group.icon) {
                    <mat-icon class="group-icon">{{ group.icon }}</mat-icon>
                  }
                  <span class="group-title">{{ group.name }}</span>
                </div>
                <mat-icon class="expand-icon" [class.expanded]="isGroupExpanded(group.name)">
                  chevron_right
                </mat-icon>
              </button>
            }

            @if (collapsed() || isGroupExpanded(group.name)) {
              <div class="nav-items-container" [class.sub-level]="!collapsed() && !group.hideHeader && group.items.length > 1">
                @for (item of group.items; track item.route) {
                  <a 
                    [routerLink]="item.route" 
                    routerLinkActive="active" 
                    class="nav-item-link"
                    [matTooltip]="collapsed() ? item.label : ''"
                    matTooltipPosition="right">
                    <mat-icon class="nav-icon">{{ item.icon }}</mat-icon>
                    @if (!collapsed()) {
                      <span class="nav-label">{{ item.label }}</span>
                    }
                  </a>
                }
              </div>
            }
          </div>
        }
      </nav>
    </aside>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
    }

    .sidebar-inner {
      display: flex;
      flex-direction: column;
      height: 100%;
      background-color: var(--sidebar-bg);
      color: var(--text-primary);
      border-right: 1px solid var(--sidebar-border);
      user-select: none;
    }

    .brand {
      height: 60px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 8px 0 16px;
      background-color: var(--sidebar-brand-bg);
      border-bottom: 1px solid var(--border-default);
      flex-shrink: 0;

      &.collapsed-brand {
        padding: 0 8px;
        justify-content: center;

        .brand-left {
          display: none;
        }
      }
    }

    .brand-left {
      display: flex;
      align-items: center;
      gap: 12px;
      overflow: hidden;
    }

    .collapse-toggle-btn {
      background: transparent;
      border: none;
      color: var(--sidebar-icon);
      width: 34px;
      height: 34px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: background-color 0.15s ease, color 0.15s ease;
      flex-shrink: 0;

      mat-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
      }

      &:hover {
        background-color: var(--sidebar-hover-bg);
        color: var(--sidebar-hover-text);
      }
    }

    .brand-icon {
      color: var(--sidebar-active-text);
      font-size: 24px;
      width: 24px;
      height: 24px;
    }

    .brand-text {
      font-size: 1.125rem;
      font-weight: 700;
      white-space: nowrap;
      letter-spacing: -0.01em;
      color: var(--text-primary);
    }

    .nav-menu {
      flex: 1;
      overflow-y: auto;
      padding: 12px 8px;
      display: flex;
      flex-direction: column;
      gap: 4px;

      &::-webkit-scrollbar {
        width: 4px;
      }
      &::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.15);
        border-radius: 4px;
      }
    }

    .nav-group {
      display: flex;
      flex-direction: column;
      margin-bottom: 12px;
    }

    .nav-group-header {
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 8px 12px;
      background: none;
      border: none;
      color: var(--sidebar-header);
      font-size: 12px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      cursor: pointer;
      border-radius: 6px;
      transition: background-color 150ms ease, color 150ms ease;

      &:hover {
        background-color: var(--sidebar-hover-bg);
        color: var(--sidebar-hover-text);
      }

      &.active-group {
        color: var(--sidebar-hover-text);
      }
    }

    .group-header-left {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .group-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      opacity: 0.8;
    }

    .group-title {
      white-space: nowrap;
    }

    .expand-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
      color: var(--sidebar-expand-icon);
      transition: transform 180ms ease, color 150ms ease;
      
      &.expanded {
        transform: rotate(90deg);
        color: var(--sidebar-active-text);
      }
    }

    .nav-items-container {
      display: flex;
      flex-direction: column;
      gap: 2px;
      margin-top: 4px;
    }

    .nav-items-container.sub-level {
      padding-left: 8px;
      border-left: 1px solid var(--border-divider);
      margin-left: 14px;
    }

    .nav-item-link {
      display: flex;
      align-items: center;
      gap: 12px;
      height: 40px;
      padding: 0 12px;
      color: var(--sidebar-text);
      text-decoration: none;
      font-size: 15px;
      font-weight: 500;
      border-radius: 10px;
      transition: all 150ms ease;
      white-space: nowrap;
      position: relative;
      letter-spacing: 0.01em;

      .nav-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
        color: var(--sidebar-icon);
        flex-shrink: 0;
        transition: color 150ms ease;
      }

      &:hover {
        background-color: var(--sidebar-hover-bg);
        color: var(--sidebar-hover-text);

        .nav-icon {
          color: var(--sidebar-hover-text);
        }
      }

      &.active {
        background-color: var(--sidebar-active-bg);
        color: var(--sidebar-active-text);
        font-weight: 600;
        
        &::before {
          content: '';
          position: absolute;
          left: 0;
          top: 0;
          bottom: 0;
          width: 3px;
          background-color: var(--sidebar-active-text);
          border-radius: 0 3px 3px 0;
        }

        .nav-icon {
          color: var(--sidebar-active-text);
        }
      }
    }
  `]
})
export class SidebarComponent implements OnInit {
  private readonly router = inject(Router);

  readonly collapsed = input<boolean>(false);
  readonly toggleCollapse = output<void>();
  readonly navigationEnded = output<void>();

  // Set of group names that are currently expanded
  readonly expandedGroups = signal<Set<string>>(new Set([
    'Overview', 'Sales', 'Inventory', 'Purchasing', 'Finance', 'HR', 'Settings'
  ]));

  readonly navGroups: NavGroup[] = [
    {
      name: 'Overview',
      hideHeader: true,
      icon: 'dashboard',
      items: [
        { label: 'Dashboard', route: '/admin/dashboard', icon: 'dashboard' }
      ]
    },
    {
      name: 'Sales',
      icon: 'trending_up',
      items: [
        { label: 'Orders', route: '/admin/orders', icon: 'shopping_cart' },
        { label: 'Customers', route: '/admin/customers', icon: 'people' }
      ]
    },
    {
      name: 'Inventory',
      icon: 'inventory',
      items: [
        { label: 'Products', route: '/admin/products', icon: 'inventory_2' },
        { label: 'Categories', route: '/admin/categories', icon: 'category' }
      ]
    },
    {
      name: 'Purchasing',
      icon: 'local_shipping',
      items: [
        { label: 'Suppliers', route: '/admin/suppliers', icon: 'store' },
        { label: 'Purchase Orders', route: '/admin/purchase-orders', icon: 'receipt' }
      ]
    },
    {
      name: 'Finance',
      icon: 'account_balance',
      items: [
        { label: 'Invoices', route: '/admin/invoices', icon: 'request_quote' }
      ]
    },
    {
      name: 'HR',
      icon: 'groups',
      items: [
        { label: 'Employees', route: '/admin/employees', icon: 'badge' },
        { label: 'Departments', route: '/admin/departments', icon: 'domain' }
      ]
    },
    {
      name: 'Settings',
      icon: 'settings',
      items: [
        { label: 'Users', route: '/admin/users', icon: 'manage_accounts' },
        { label: 'Roles', route: '/admin/roles', icon: 'security' }
      ]
    }
  ];

  ngOnInit(): void {
    this.autoExpandActiveGroup(this.router.url);

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.autoExpandActiveGroup(event.urlAfterRedirects || event.url);
      this.navigationEnded.emit();
    });
  }

  isGroupExpanded(groupName: string): boolean {
    return this.expandedGroups().has(groupName);
  }

  toggleGroup(groupName: string): void {
    this.expandedGroups.update(set => {
      const next = new Set(set);
      if (next.has(groupName)) {
        next.delete(groupName);
      } else {
        next.add(groupName);
      }
      return next;
    });
  }

  isGroupActive(group: NavGroup): boolean {
    const currentUrl = this.router.url;
    return group.items.some(item => currentUrl.startsWith(item.route));
  }

  private autoExpandActiveGroup(url: string): void {
    for (const group of this.navGroups) {
      if (group.items.some(item => url.startsWith(item.route))) {
        if (!this.isGroupExpanded(group.name)) {
          this.expandedGroups.update(set => {
            const next = new Set(set);
            next.add(group.name);
            return next;
          });
        }
        break;
      }
    }
  }
}
