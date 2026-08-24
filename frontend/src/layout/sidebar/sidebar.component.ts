import { Component, inject, input, output, signal, OnInit, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { filter } from 'rxjs/operators';
import { NavGroup, NavItem } from '../models/nav.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, MatIconModule, MatTooltipModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

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
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe((event) => {
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
