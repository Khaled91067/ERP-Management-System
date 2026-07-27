import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

import { UsersService } from '../users.service';
import { RolesService, Role } from '@features/roles/roles.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatSelectModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <app-page-header
        [title]="isEditMode() ? 'Edit User' : 'New User'"
        [breadcrumbs]="[
          { label: 'Settings' },
          { label: 'Users', link: '/admin/users' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="userForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>First Name</mat-label>
                <input matInput formControlName="firstName" placeholder="John">
                @if (userForm.controls.firstName.hasError('required')) {
                  <mat-error>First name is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Last Name</mat-label>
                <input matInput formControlName="lastName" placeholder="Doe">
                @if (userForm.controls.lastName.hasError('required')) {
                  <mat-error>Last name is required</mat-error>
                }
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email Address</mat-label>
              <input matInput type="email" formControlName="email" placeholder="john.doe@company.com">
              @if (userForm.controls.email.hasError('required')) {
                <mat-error>Email is required</mat-error>
              }
              @if (userForm.controls.email.hasError('email')) {
                <mat-error>Must be a valid email</mat-error>
              }
            </mat-form-field>

            @if (!isEditMode()) {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Password</mat-label>
                <input matInput type="password" formControlName="password">
                @if (userForm.controls.password?.hasError('required')) {
                  <mat-error>Password is required for new users</mat-error>
                }
                @if (userForm.controls.password?.hasError('minlength')) {
                  <mat-error>Password must be at least 6 characters</mat-error>
                }
              </mat-form-field>
            }

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Role</mat-label>
              <mat-select formControlName="roleId">
                @for (role of availableRoles(); track role.id) {
                  <mat-option [value]="role.id">{{ role.name }}</mat-option>
                }
              </mat-select>
              @if (userForm.controls.roleId?.hasError('required')) {
                <mat-error>Role is required</mat-error>
              }
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/users'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="userForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save User' }}
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

    .form-row {
      display: flex;
      gap: 16px;
    }

    @media (max-width: 600px) {
      .form-row {
        flex-direction: column;
        gap: 0;
      }
    }

    .half-width {
      flex: 1;
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
export class UserFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly usersService = inject(UsersService);
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isEditMode = signal(false);
  readonly isSaving = signal(false);
  readonly availableRoles = signal<Role[]>([]);
  userId: number | null = null;
  initialRoleId: number | null = null;

  userForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    roleId: [0 as number | null, Validators.required], // Will be set dynamically
    password: ['']
  });

  ngOnInit(): void {
    this.loadRoles();
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      this.isEditMode.set(true);
      this.userId = +id;
      // In edit mode, password is not required
      this.userForm.controls.password.clearValidators();
      this.userForm.controls.password.updateValueAndValidity();
      this.loadUser(this.userId);
    } else {
      // In create mode, password is required
      this.userForm.controls.password.setValidators([Validators.required, Validators.minLength(6)]);
      this.userForm.controls.password.updateValueAndValidity();
    }
  }

  private loadRoles(): void {
    this.rolesService.getRoles({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.availableRoles.set(res.items),
      error: () => this.notification.error('Failed to load roles')
    });
  }

  private loadUser(id: number): void {
    this.usersService.getUser(id).subscribe({
      next: (user) => {
        // Need to match role name back to role id from available roles, 
        // assuming the API returns the role name as string in user.role
        let roleId = null;
        if (user.role) {
          const matchedRole = this.availableRoles().find(r => r.name === user.role);
          if (matchedRole) {
            roleId = matchedRole.id;
            this.initialRoleId = roleId;
          }
        }
        
        this.userForm.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          roleId: roleId
        });
      },
      error: () => {
        this.notification.error('Failed to load user details');
        this.router.navigate(['/admin/users']);
      }
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) return;

    this.isSaving.set(true);
    const data = this.userForm.getRawValue();

    if (this.isEditMode() && this.userId) {
      const updateData = {
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email
      };
      
      this.usersService.updateUser(this.userId, updateData).subscribe({
        next: () => {
          // If role changed, make a separate call
          if (data.roleId && data.roleId !== this.initialRoleId) {
            this.usersService.changeRole(this.userId!, data.roleId).subscribe({
              next: () => this.finishSave(),
              error: () => {
                this.notification.error('User updated, but failed to change role');
                this.isSaving.set(false);
              }
            });
          } else {
            this.finishSave();
          }
        },
        error: () => {
          this.notification.error('Failed to update user');
          this.isSaving.set(false);
        }
      });
    } else {
      const createData = {
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        password: data.password,
        roleId: data.roleId as number
      };
      
      this.usersService.createUser(createData).subscribe({
        next: () => this.finishSave(),
        error: () => {
          this.notification.error('Failed to create user');
          this.isSaving.set(false);
        }
      });
    }
  }
  
  private finishSave(): void {
    this.notification.success(`User successfully ${this.isEditMode() ? 'updated' : 'created'}`);
    this.router.navigate(['/admin/users']);
  }
}
