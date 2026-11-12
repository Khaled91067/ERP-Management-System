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
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';

export interface PermissionItem {
  key: string;
  labelKey: string;
}

export interface PermissionModule {
  nameKey: string;
  icon: string;
  permissions: PermissionItem[];
}

export const PERMISSION_MODULES: PermissionModule[] = [
  {
    nameKey: 'roles.modules.customers',
    icon: 'people',
    permissions: [
      { key: 'Customers.Read', labelKey: 'roles.permissionsMap.Customers.Read' },
      { key: 'Customers.Write', labelKey: 'roles.permissionsMap.Customers.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.orders',
    icon: 'shopping_cart',
    permissions: [
      { key: 'Orders.Read', labelKey: 'roles.permissionsMap.Orders.Read' },
      { key: 'Orders.Write', labelKey: 'roles.permissionsMap.Orders.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.productsInventory',
    icon: 'inventory_2',
    permissions: [
      { key: 'Products.Read', labelKey: 'roles.permissionsMap.Products.Read' },
      { key: 'Products.Write', labelKey: 'roles.permissionsMap.Products.Write' },
      { key: 'Products.AdjustStock', labelKey: 'roles.permissionsMap.Products.AdjustStock' }
    ]
  },
  {
    nameKey: 'roles.modules.categories',
    icon: 'category',
    permissions: [
      { key: 'Categories.Read', labelKey: 'roles.permissionsMap.Categories.Read' },
      { key: 'Categories.Write', labelKey: 'roles.permissionsMap.Categories.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.suppliers',
    icon: 'store',
    permissions: [
      { key: 'Suppliers.Read', labelKey: 'roles.permissionsMap.Suppliers.Read' },
      { key: 'Suppliers.Write', labelKey: 'roles.permissionsMap.Suppliers.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.purchaseOrders',
    icon: 'receipt',
    permissions: [
      { key: 'PurchaseOrders.Read', labelKey: 'roles.permissionsMap.PurchaseOrders.Read' },
      { key: 'PurchaseOrders.Write', labelKey: 'roles.permissionsMap.PurchaseOrders.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.invoicesBilling',
    icon: 'request_quote',
    permissions: [
      { key: 'Invoices.Read', labelKey: 'roles.permissionsMap.Invoices.Read' },
      { key: 'Invoices.Write', labelKey: 'roles.permissionsMap.Invoices.Write' },
      { key: 'Invoices.Send', labelKey: 'roles.permissionsMap.Invoices.Send' },
      { key: 'Invoices.Pay', labelKey: 'roles.permissionsMap.Invoices.Pay' }
    ]
  },
  {
    nameKey: 'roles.modules.employees',
    icon: 'badge',
    permissions: [
      { key: 'Employees.Read', labelKey: 'roles.permissionsMap.Employees.Read' },
      { key: 'Employees.Write', labelKey: 'roles.permissionsMap.Employees.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.departments',
    icon: 'domain',
    permissions: [
      { key: 'Departments.Read', labelKey: 'roles.permissionsMap.Departments.Read' },
      { key: 'Departments.Write', labelKey: 'roles.permissionsMap.Departments.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.users',
    icon: 'manage_accounts',
    permissions: [
      { key: 'Users.Read', labelKey: 'roles.permissionsMap.Users.Read' },
      { key: 'Users.Write', labelKey: 'roles.permissionsMap.Users.Write' }
    ]
  },
  {
    nameKey: 'roles.modules.roles',
    icon: 'security',
    permissions: [
      { key: 'Roles.Read', labelKey: 'roles.permissionsMap.Roles.Read' },
      { key: 'Roles.Write', labelKey: 'roles.permissionsMap.Roles.Write' }
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
    PageHeaderComponent,
    TranslatePipe
  ],
  templateUrl: './role-form.component.html',
  styleUrl: './role-form.component.scss'
})
export class RoleFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
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
    this.formValidation.prepareFormForSubmission(this.roleForm);
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
          this.notification.success(this.i18n.translate('roles.updatedSuccess'));
          this.router.navigate(['/admin/roles']);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      this.rolesService.createRole(payload).subscribe({
        next: () => {
          this.notification.success(this.i18n.translate('roles.createdSuccess'));
          this.router.navigate(['/admin/roles']);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }
}
