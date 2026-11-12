import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { SuppliersService } from '../suppliers.service';
import { FormValidationService } from '@core/errors/form-validation.service';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { TranslatePipe } from '@shared/pipes/translate.pipe';

@Component({
  selector: 'app-supplier-form',
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
  templateUrl: './supplier-form.component.html',
  styleUrl: './supplier-form.component.scss'
})
export class SupplierFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly suppliersService = inject(SuppliersService);
  private readonly notification = inject(NotificationService);
  private readonly formValidation = inject(FormValidationService);
  private readonly i18n = inject(TranslationService);
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
    paymentTerms: ['Net 30', Validators.required]
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
        this.router.navigate(['/admin/suppliers']);
      }
    });
  }

  onSubmit(): void {
    this.formValidation.prepareFormForSubmission(this.supplierForm);
    if (this.supplierForm.invalid) return;

    this.isSaving.set(true);
    const data = this.supplierForm.getRawValue();

    const request$: Observable<unknown> = this.isEditMode() && this.supplierId
      ? this.suppliersService.updateSupplier(this.supplierId, { id: this.supplierId, ...data })
      : this.suppliersService.createSupplier(data);

    request$.subscribe({
      next: () => {
        const msgKey = this.isEditMode() ? 'suppliers.updatedSuccess' : 'suppliers.createdSuccess';
        this.notification.success(this.i18n.translate(msgKey));
        this.router.navigate(['/admin/suppliers']);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
