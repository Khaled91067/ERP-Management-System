import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { RolesService } from '../roles.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        [title]="isEditMode() ? 'Edit Role' : 'New Role'"
        [breadcrumbs]="[
          { label: 'Settings' },
          { label: 'Roles', link: '/admin/roles' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="roleForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Role Name</mat-label>
              <input matInput formControlName="name" placeholder="e.g., Manager">
              @if (roleForm.controls.name.hasError('required')) {
                <mat-error>Role name is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Permissions (comma separated)</mat-label>
              <textarea matInput formControlName="permissions" rows="3" placeholder="e.g., read:users, write:orders"></textarea>
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/roles'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="roleForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save Role' }}
              </button>
            </div>
            
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .form-card {
      max-width: 600px;
      padding: 24px 16px;
      border: 1px solid var(--border-color);
      border-radius: 12px;
      background-color: var(--surface-card);
    }

    .form-layout {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .full-width {
      width: 100%;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 16px;
    
      flex-wrap: wrap;
    }
  `]
})
export class RoleFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isEditMode = signal(false);
  readonly isSaving = signal(false);
  roleId: number | null = null;

  readonly roleForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    permissions: ['']
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.roleId = +id;
      this.loadRole(this.roleId);
    }
  }

  private loadRole(id: number): void {
    this.rolesService.getRole(id).subscribe({
      next: (role) => {
        this.roleForm.patchValue({
          name: role.name,
          permissions: role.permissions || ''
        });
      },
      error: () => {
        this.notification.error('Failed to load role details');
        this.router.navigate(['/admin/roles']);
      }
    });
  }

  onSubmit(): void {
    if (this.roleForm.invalid) return;

    this.isSaving.set(true);
    const data = this.roleForm.getRawValue();

    const request$ = this.isEditMode() && this.roleId
      ? this.rolesService.updateRole(this.roleId, data)
      : this.rolesService.createRole(data);

    (request$ as any).subscribe({
      next: () => {
        this.notification.success(`Role successfully ${this.isEditMode() ? 'updated' : 'created'}`);
        this.router.navigate(['/admin/roles']);
      },
      error: () => {
        this.notification.error(`Failed to ${this.isEditMode() ? 'update' : 'create'} role`);
        this.isSaving.set(false);
      }
    });
  }
}
