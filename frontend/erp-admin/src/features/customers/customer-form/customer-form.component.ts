import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { CustomersService } from '../customers.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';

@Component({
  selector: 'app-customer-form',
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
        [title]="isEditMode() ? 'Edit Customer' : 'New Customer'"
        [breadcrumbs]="[
          { label: 'Sales' },
          { label: 'Customers', link: '/admin/customers' },
          { label: isEditMode() ? 'Edit' : 'New' }
        ]"
      />

      <mat-card class="form-card mat-elevation-z0">
        <mat-card-content>
          <form [formGroup]="customerForm" (ngSubmit)="onSubmit()" class="form-layout">
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Customer Name</mat-label>
                <input matInput formControlName="name" placeholder="Acme Corp">
                @if (customerForm.controls.name.hasError('required')) {
                  <mat-error>Name is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Email Address</mat-label>
                <input matInput type="email" formControlName="email" placeholder="contact@acme.com">
                @if (customerForm.controls.email.hasError('required')) {
                  <mat-error>Email is required</mat-error>
                }
                @if (customerForm.controls.email.hasError('email')) {
                  <mat-error>Must be a valid email</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Phone</mat-label>
                <input matInput formControlName="phone" placeholder="+1 (555) 000-0000">
                @if (customerForm.controls.phone.hasError('required')) {
                  <mat-error>Phone is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Tax ID</mat-label>
                <input matInput formControlName="taxId" placeholder="AB-12345678">
                @if (customerForm.controls.taxId.hasError('required')) {
                  <mat-error>Tax ID is required</mat-error>
                }
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Address</mat-label>
              <textarea matInput formControlName="address" rows="2" placeholder="123 Business Rd, Suite 100"></textarea>
              @if (customerForm.controls.address.hasError('required')) {
                <mat-error>Address is required</mat-error>
              }
            </mat-form-field>
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>City</mat-label>
                <input matInput formControlName="city" placeholder="New York">
                @if (customerForm.controls.city.hasError('required')) {
                  <mat-error>City is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Country</mat-label>
                <input matInput formControlName="country" placeholder="USA">
                @if (customerForm.controls.country.hasError('required')) {
                  <mat-error>Country is required</mat-error>
                }
              </mat-form-field>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="router.navigate(['/admin/customers'])">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="customerForm.invalid || isSaving()">
                {{ isSaving() ? 'Saving...' : 'Save Customer' }}
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
export class CustomerFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);

  readonly isEditMode = signal(false);
  readonly isSaving = signal(false);
  customerId: number | null = null;

  readonly customerForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    taxId: ['', Validators.required],
    address: ['', Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.customerId = +id;
      this.loadCustomer(this.customerId);
    }
  }

  private loadCustomer(id: number): void {
    this.customersService.getCustomer(id).subscribe({
      next: (customer) => {
        this.customerForm.patchValue(customer);
      },
      error: () => {
        this.notification.error('Failed to load customer details');
        this.router.navigate(['/admin/customers']);
      }
    });
  }

  onSubmit(): void {
    if (this.customerForm.invalid) return;

    this.isSaving.set(true);
    const data = this.customerForm.getRawValue();

    const request$ = this.isEditMode() && this.customerId
      ? this.customersService.updateCustomer(this.customerId, { id: this.customerId, ...data })
      : this.customersService.createCustomer(data);

    (request$ as any).subscribe({
      next: () => {
        // We use string concatenation to avoid template literal issues in generation
        const msg = 'Customer successfully ' + (this.isEditMode() ? 'updated' : 'created');
        this.notification.success(msg);
        this.router.navigate(['/admin/customers']);
      },
      error: () => {
        const msg = 'Failed to ' + (this.isEditMode() ? 'update' : 'create') + ' customer';
        this.notification.error(msg);
        this.isSaving.set(false);
      }
    });
  }
}
