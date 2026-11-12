import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

import { UsersService } from '../users.service';
import { RolesService } from '@features/roles/roles.service';
import { Role } from '@features/roles/models/role.model';
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';

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
    PageHeaderComponent,
    TranslatePipe
  ],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss'
})
export class UserFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly usersService = inject(UsersService);
  private readonly rolesService = inject(RolesService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  readonly i18n = inject(TranslationService);
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
    roleId: [null as number | null, Validators.required],
    password: ['']
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.isEditMode.set(true);
      this.userId = +id;
      this.userForm.controls.password.clearValidators();
      this.userForm.controls.password.updateValueAndValidity();
      this.loadRolesAndUser(this.userId);
    } else {
      this.loadRoles();
      this.userForm.controls.password.setValidators([Validators.required, Validators.minLength(6)]);
      this.userForm.controls.password.updateValueAndValidity();
    }
  }

  private loadRoles(): void {
    this.rolesService.getRoles({ page: 1, pageSize: 100 }).subscribe({
      next: (res) => this.availableRoles.set(res.items)
    });
  }

  private loadRolesAndUser(id: number): void {
    forkJoin({
      roles: this.rolesService.getRoles({ page: 1, pageSize: 100 }),
      user: this.usersService.getUser(id)
    }).subscribe({
      next: ({ roles, user }) => {
        this.availableRoles.set(roles.items);

        let roleId = user.roleId || null;
        const roleName = user.roleName || user.role;
        if (!roleId && roleName) {
          const matchedRole = roles.items.find(r => r.name === roleName);
          if (matchedRole) {
            roleId = matchedRole.id;
          }
        }
        if (roleId) {
          this.initialRoleId = roleId;
        }

        this.userForm.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          roleId: roleId
        });
      },
      error: () => {
        this.router.navigate(['/admin/users']);
      }
    });
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.userForm);
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
          if (data.roleId && data.roleId !== this.initialRoleId) {
            this.usersService.changeRole(this.userId!, data.roleId).subscribe({
              next: () => this.finishSave(),
              error: () => {
                this.notification.error(this.i18n.translate('users.roleChangeFailed'));
                this.isSaving.set(false);
              }
            });
          } else {
            this.finishSave();
          }
        },
        error: () => {
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
          this.isSaving.set(false);
        }
      });
    }
  }

  private finishSave(): void {
    const msgKey = this.isEditMode() ? 'users.updatedSuccess' : 'users.createdSuccess';
    this.notification.success(this.i18n.translate(msgKey));
    this.router.navigate(['/admin/users']);
  }
}
