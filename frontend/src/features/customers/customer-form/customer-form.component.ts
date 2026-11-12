import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { CustomersService } from '../customers.service';
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    PageHeaderComponent,
    TranslatePipe
  ],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.scss'
})
export class CustomerFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly customersService = inject(CustomersService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
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
        this.router.navigate(['/admin/customers']);
      }
    });
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.customerForm);
    if (this.customerForm.invalid) return;

    this.isSaving.set(true);
    const data = this.customerForm.getRawValue();

    const request$: Observable<unknown> = this.isEditMode() && this.customerId
      ? this.customersService.updateCustomer(this.customerId, { id: this.customerId, ...data })
      : this.customersService.createCustomer(data);

    request$.subscribe({
      next: () => {
        const msgKey = this.isEditMode() ? 'customers.updatedSuccess' : 'customers.createdSuccess';
        this.notification.success(this.i18n.translate(msgKey));
        this.router.navigate(['/admin/customers']);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
