import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { RolesService } from '../roles.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

export interface PermissionItem {
  key: string;
  label: string;
}

export interface PermissionModule {
  name: string;
  icon: string;
  permissions: PermissionItem[];
}

export const PERMISSION_MODULES: PermissionModule[] = [
  {
    name: 'Customers',
    icon: 'people',
    permissions: [
      { key: 'Customers.Read', label: 'View Customers' },
      { key: 'Customers.Write', label: 'Manage Customers' }
    ]
  },
  {
    name: 'Orders',
    icon: 'shopping_cart',
    permissions: [
      { key: 'Orders.Read', label: 'View Orders' },
      { key: 'Orders.Write', label: 'Manage Orders' }
    ]
  },
  {
    name: 'Products & Inventory',
    icon: 'inventory_2',
    permissions: [
      { key: 'Products.Read', label: 'View Products' },
      { key: 'Products.Write', label: 'Manage Products' },
      { key: 'Products.AdjustStock', label: 'Adjust Stock' }
    ]
  },
  {
    name: 'Categories',
    icon: 'category',
    permissions: [
      { key: 'Categories.Read', label: 'View Categories' },
      { key: 'Categories.Write', label: 'Manage Categories' }
    ]
  },
  {
    name: 'Suppliers',
    icon: 'store',
    permissions: [
      { key: 'Suppliers.Read', label: 'View Suppliers' },
      { key: 'Suppliers.Write', label: 'Manage Suppliers' }
    ]
  },
  {
    name: 'Purchase Orders',
    icon: 'receipt',
    permissions: [
      { key: 'PurchaseOrders.Read', label: 'View Purchase Orders' },
      { key: 'PurchaseOrders.Write', label: 'Manage Purchase Orders' }
    ]
  },
  {
    name: 'Invoices & Billing',
    icon: 'request_quote',
    permissions: [
      { key: 'Invoices.Read', label: 'View Invoices' },
      { key: 'Invoices.Write', label: 'Manage Invoices' },
      { key: 'Invoices.Send', label: 'Send Invoices' },
      { key: 'Invoices.Pay', label: 'Pay Invoices' }
    ]
  },
  {
    name: 'Employees',
    icon: 'badge',
    permissions: [
      { key: 'Employees.Read', label: 'View Employees' },
      { key: 'Employees.Write', label: 'Manage Employees' }
    ]
  },
  {
    name: 'Departments',
    icon: 'domain',
    permissions: [
      { key: 'Departments.Read', label: 'View Departments' },
      { key: 'Departments.Write', label: 'Manage Departments' }
    ]
  },
  {
    name: 'Users',
    icon: 'manage_accounts',
    permissions: [
      { key: 'Users.Read', label: 'View Users' },
      { key: 'Users.Write', label: 'Manage Users' }
    ]
  },
  {
    name: 'Roles',
    icon: 'security',
    permissions: [
      { key: 'Roles.Read', label: 'View Roles' },
      { key: 'Roles.Write', label: 'Manage Roles' }
    ]
  }
];

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatCheckboxModule,
    MatIconModule,
    MatDividerModule,
    MatTooltipModule,
    PageHeaderComponent
  ],
  templateUrl: './role-form.component.html',
  styleUrl: './role-form.component.scss'
})
export class RoleFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  readonly roleId = signal<number | null>(null);

  readonly permissionModules = PERMISSION_MODULES;
  readonly selectedPermissions = signal<Set<string>>(new Set());

  readonly totalPermissionCount = computed(() => {
    return this.permissionModules.reduce((acc, m) => acc + m.permissions.length, 0);
  });

  readonly selectedCount = computed(() => this.selectedPermissions().size);

  readonly roleForm = this.fb.nonNullable.group({
    name: ['', Validators.required]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.roleId.set(+id);
      this.loadRole(+id);
    }
  }

  private loadRole(id: number): void {
    this.rolesService.getRole(id).subscribe({
      next: (role) => {
        this.roleForm.patchValue({
          name: role.name
        });

        if (role.permissions) {
          try {
            const parsed = JSON.parse(role.permissions);
            if (Array.isArray(parsed)) {
              this.selectedPermissions.set(new Set(parsed));
            }
          } catch {
            this.selectedPermissions.set(new Set());
          }
        }
      },
      error: () => {
        this.notification.error('Failed to load role details');
        this.router.navigate(['/admin/roles']);
      }
    });
  }

  isPermissionChecked(key: string): boolean {
    return this.selectedPermissions().has(key);
  }

  togglePermission(key: string): void {
    this.selectedPermissions.update(set => {
      const next = new Set(set);
      if (next.has(key)) {
        next.delete(key);
      } else {
        next.add(key);
      }
      return next;
    });
  }

  isModuleAllChecked(module: PermissionModule): boolean {
    return module.permissions.every(p => this.selectedPermissions().has(p.key));
  }

  isModulePartiallyChecked(module: PermissionModule): boolean {
    const checkedCount = module.permissions.filter(p => this.selectedPermissions().has(p.key)).length;
    return checkedCount > 0 && checkedCount < module.permissions.length;
  }

  toggleModule(module: PermissionModule): void {
    const allChecked = this.isModuleAllChecked(module);
    this.selectedPermissions.update(set => {
      const next = new Set(set);
      for (const p of module.permissions) {
        if (allChecked) {
          next.delete(p.key);
        } else {
          next.add(p.key);
        }
      }
      return next;
    });
  }

  selectAll(): void {
    const all = new Set<string>();
    for (const m of this.permissionModules) {
      for (const p of m.permissions) {
        all.add(p.key);
      }
    }
    this.selectedPermissions.set(all);
  }

  deselectAll(): void {
    this.selectedPermissions.set(new Set());
  }

  onSubmit(): void {
    if (this.roleForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.roleForm.getRawValue();
    const permissionsArray = Array.from(this.selectedPermissions());
    const payload = {
      name: formValue.name,
      permissions: JSON.stringify(permissionsArray)
    };

    if (this.isEditMode() && this.roleId()) {
      this.rolesService.updateRole(this.roleId()!, payload).subscribe({
        next: () => {
          this.notification.success('Role updated successfully');
          this.router.navigate(['/admin/roles']);
        },
        error: () => {
          this.notification.error('Failed to update role');
          this.isSaving.set(false);
        }
      });
    } else {
      this.rolesService.createRole(payload).subscribe({
        next: () => {
          this.notification.success('Role created successfully');
          this.router.navigate(['/admin/roles']);
        },
        error: () => {
          this.notification.error('Failed to create role');
          this.isSaving.set(false);
        }
      });
    }
  }
}
