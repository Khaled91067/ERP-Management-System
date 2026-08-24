import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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

  readonly roleForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['']
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
          name: role.name,
          description: role.description || role.permissions || ''
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
    const formValue = this.roleForm.getRawValue();
    const payload = {
      name: formValue.name,
      permissions: formValue.description || ''
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
