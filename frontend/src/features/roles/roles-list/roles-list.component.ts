import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { RolesService } from '../roles.service';
import { Role } from '../models/role.model';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule,
    MatTooltipModule,
    PageHeaderComponent
  ],
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.scss'
})
export class RolesListComponent implements OnInit {
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  readonly columns = ['id', 'name', 'permissions', 'actions'];
  readonly roles = signal<Role[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');
  readonly maxVisiblePermissions = 3;

  private static readonly PERMISSION_LABEL_MAP: Record<string, string> = {
    'Customers.Read': 'Customers Read',
    'Customers.Write': 'Customers Write',
    'Orders.Read': 'Orders Read',
    'Orders.Write': 'Orders Write',
    'Products.Read': 'Products Read',
    'Products.Write': 'Products Write',
    'Products.AdjustStock': 'Products Adjust Stock',
    'Categories.Read': 'Categories Read',
    'Categories.Write': 'Categories Write',
    'Suppliers.Read': 'Suppliers Read',
    'Suppliers.Write': 'Suppliers Write',
    'PurchaseOrders.Read': 'Purchase Orders Read',
    'PurchaseOrders.Write': 'Purchase Orders Write',
    'Invoices.Read': 'Invoices Read',
    'Invoices.Write': 'Invoices Write',
    'Invoices.Send': 'Invoices Send',
    'Invoices.Pay': 'Invoices Pay',
    'Employees.Read': 'Employees Read',
    'Employees.Write': 'Employees Write',
    'Departments.Read': 'Departments Read',
    'Departments.Write': 'Departments Write',
    'Users.Read': 'Users Read',
    'Users.Write': 'Users Write',
    'Roles.Read': 'Roles Read',
    'Roles.Write': 'Roles Write'
  };

  ngOnInit(): void {
    this.loadRoles();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadRoles();
      });
  }

  loadRoles(): void {
    this.isLoading.set(true);
    
    this.rolesService.getRoles({
      page: this.pageIndex() + 1,
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined
    }).subscribe({
      next: (result) => {
        this.roles.set(result.items);
        this.totalItems.set(result.total);
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load roles');
        this.isLoading.set(false);
      }
    });
  }

  getParsedPermissions(role: Role): string[] {
    if (!role.permissions) return [];
    try {
      const parsed = typeof role.permissions === 'string'
        ? JSON.parse(role.permissions)
        : role.permissions;
      return Array.isArray(parsed)
        ? parsed.filter((p): p is string => typeof p === 'string' && p.trim().length > 0)
        : [];
    } catch {
      return [];
    }
  }

  isFullAccess(role: Role): boolean {
    if (role.name?.trim().toLowerCase() === 'administrator') {
      return true;
    }
    return this.getParsedPermissions(role).length >= 23;
  }

  formatPermissionLabel(permission: string): string {
    if (!permission) return '';
    if (RolesListComponent.PERMISSION_LABEL_MAP[permission]) {
      return RolesListComponent.PERMISSION_LABEL_MAP[permission];
    }
    return permission
      .split('.')
      .map(part => part.replace(/([a-z])([A-Z])/g, '$1 $2'))
      .join(' ');
  }

  getVisiblePermissions(role: Role): string[] {
    const perms = this.getParsedPermissions(role);
    return perms.slice(0, this.maxVisiblePermissions).map(p => this.formatPermissionLabel(p));
  }

  getHiddenPermissionsCount(role: Role): number {
    const perms = this.getParsedPermissions(role);
    return Math.max(0, perms.length - this.maxVisiblePermissions);
  }

  getHiddenPermissionsTooltip(role: Role): string {
    const perms = this.getParsedPermissions(role);
    if (perms.length <= this.maxVisiblePermissions) return '';
    const hidden = perms.slice(this.maxVisiblePermissions).map(p => this.formatPermissionLabel(p));
    return hidden.join(', ');
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadRoles();
  }

  deleteRole(role: Role): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Role',
        message: `Are you sure you want to delete the role "${role.name}"?`,
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.rolesService.deleteRole(role.id).subscribe({
          next: () => {
            this.notification.success('Role deleted successfully');
            this.loadRoles();
          },
          error: () => this.notification.error('Failed to delete role')
        });
      }
    });
  }
}
