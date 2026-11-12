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
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';

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
    PageHeaderComponent,
    TranslatePipe
  ],
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.scss'
})
export class RolesListComponent implements OnInit {
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  readonly i18n = inject(TranslationService);
  readonly router = inject(Router);

  readonly columns = ['id', 'name', 'permissions', 'actions'];
  readonly roles = signal<Role[]>([]);
  readonly totalItems = signal(0);
  readonly pageSize = signal(20);
  readonly pageIndex = signal(0);
  readonly isLoading = signal(false);

  readonly searchControl = new FormControl('');
  readonly maxVisiblePermissions = 3;

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
    const translationKey = `roles.permissionLabels.${permission}`;
    const translated = this.i18n.translate(translationKey);
    if (translated !== translationKey) {
      return translated;
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
        title: this.i18n.translate('roles.deleteTitle'),
        message: this.i18n.translate('roles.deleteMessage', { name: role.name }),
        danger: true
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.rolesService.deleteRole(role.id).subscribe({
          next: () => {
            this.notification.success(this.i18n.translate('roles.deletedSuccess'));
            this.loadRoles();
          }
        });
      }
    });
  }
}
