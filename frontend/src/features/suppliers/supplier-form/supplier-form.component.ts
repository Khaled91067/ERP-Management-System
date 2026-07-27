import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { SuppliersService } from '../suppliers.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-supplier-form',
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
        [title]="isEditMode() ? 'Edit Supplier' : 'New Supplier'"
        [breadcrumbs]="[
          { label: 'Purchasing' },
          { label: 'Suppliers', link: '/admin/suppliers' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="supplierForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Company Name</mat-label>
                <input matInput formControlName="companyName" placeholder="Global Tech Supplies">
                @if (supplierForm.controls.companyName.hasError('required')) {
                  <mat-error>Company name is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Contact Name</mat-label>
                <input matInput formControlName="contactName" placeholder="Jane Doe">
                @if (supplierForm.controls.contactName.hasError('required')) {
                  <mat-error>Contact name is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Email Address</mat-label>
                <input matInput type="email" formControlName="email" placeholder="contact@globaltech.com">
                @if (supplierForm.controls.email.hasError('required')) {
                  <mat-error>Email is required</mat-error>
                }
                @if (supplierForm.controls.email.hasError('email')) {
                  <mat-error>Must be a valid email</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Phone</mat-label>
                <input matInput formControlName="phone" placeholder="+1 (555) 123-4567">
                @if (supplierForm.controls.phone.hasError('required')) {
                  <mat-error>Phone is required</mat-error>
                }
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Payment Terms</mat-label>
              <textarea matInput formControlName="paymentTerms" rows="2" placeholder="Net 30, Wire Transfer only..."></textarea>
              @if (supplierForm.controls.paymentTerms.hasError('required')) {
                <mat-error>Payment terms are required</mat-error>
              }
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/suppliers'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="supplierForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save Supplier' }}
              </button>
            </div>
            
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .form-card {
      max-width: 800px;
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
export class SupplierFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly suppliersService = inject(SuppliersService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isEditMode = signal(false);
  readonly isSaving = signal(false);
  supplierId: number | null = null;

  readonly supplierForm = this.fb.nonNullable.group({
    companyName: ['', Validators.required],
    contactName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    paymentTerms: ['', Validators.required]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.supplierId = +id;
      this.loadSupplier(this.supplierId);
    }
  }

  private loadSupplier(id: number): void {
    this.suppliersService.getSupplier(id).subscribe({
      next: (supplier) => {
        this.supplierForm.patchValue(supplier);
      },
      error: () => {
        this.notification.error('Failed to load supplier details');
        this.router.navigate(['/admin/suppliers']);
      }
    });
  }

  onSubmit(): void {
    if (this.supplierForm.invalid) return;

    this.isSaving.set(true);
    const data = this.supplierForm.getRawValue();

    const request$ = this.isEditMode() && this.supplierId
      ? this.suppliersService.updateSupplier(this.supplierId, { id: this.supplierId, ...data })
      : this.suppliersService.createSupplier(data);

    (request$ as any).subscribe({
      next: () => {
        const msg = 'Supplier successfully ' + (this.isEditMode() ? 'updated' : 'created');
        this.notification.success(msg);
        this.router.navigate(['/admin/suppliers']);
      },
      error: () => {
        const msg = 'Failed to ' + (this.isEditMode() ? 'update' : 'create') + ' supplier';
        this.notification.error(msg);
        this.isSaving.set(false);
      }
    });
  }
}
