import { Injectable, inject } from '@angular/core';
import { AbstractControl, FormArray, FormGroup } from '@angular/forms';
import { take } from 'rxjs';
import { TranslationService } from '@core/i18n/translation.service';
import { NormalizedApiError } from '@core/models/api-error.model';

export interface FormValidationResult {
  handledByFields: boolean;
  mappedFields: string[];
  unmappedErrors: string[];
}

const FIELD_NAME_ALIASES: Record<string, string> = {
  expecteddelivery: 'expectedDeliveryDate',
  expecteddeliverydate: 'expectedDeliveryDate',
  initialstockquantity: 'stockQuantity',
  stockquantity: 'stockQuantity',
  companyname: 'companyName',
  contactname: 'contactName',
  paymentterms: 'paymentTerms',
  paymentmethod: 'paymentMethod',
  shippingaddress: 'shippingAddress',
  customerid: 'customerId',
  supplierid: 'supplierId',
  categoryid: 'categoryId',
  departmentid: 'departmentId',
  roleid: 'roleId',
  unitprice: 'unitPrice',
  costprice: 'costPrice',
  unitcost: 'unitCost',
  reorderlevel: 'reorderLevel',
  taxid: 'taxId',
  taxrate: 'taxRate',
  duedate: 'dueDate',
  hiredate: 'hireDate',
  firstname: 'firstName',
  lastname: 'lastName',
  productid: 'productId'
};

@Injectable({
  providedIn: 'root'
})
export class FormValidationService {
  private readonly i18n = inject(TranslationService);
  private activeForm: FormGroup | null = null;

  /**
   * Registers the currently submitting FormGroup so that centralized error handling
   * can automatically bind field-level ValidationProblemDetails errors and suppress
   * redundant global toast notifications.
   */
  prepareFormForSubmission(form: FormGroup): void {
    this.activeForm = form;
  }

  getActiveForm(): FormGroup | null {
    return this.activeForm;
  }

  clearActiveForm(form?: FormGroup): void {
    if (!form || this.activeForm === form) {
      this.activeForm = null;
    }
  }

  /**
   * Maps field-level validation errors from NormalizedApiError (or HttpErrorResponse)
   * onto the corresponding Angular FormGroup controls, including nested groups and FormArrays.
   */
  applyServerErrors(
    form: FormGroup,
    errorOrNormalized: NormalizedApiError | { normalizedError?: NormalizedApiError; error?: unknown } | null | undefined
  ): FormValidationResult {
    this.clearActiveForm(form);

    const fieldErrors = this.extractFieldErrors(errorOrNormalized);
    const entries = Object.entries(fieldErrors);

    if (entries.length === 0) {
      return {
        handledByFields: false,
        mappedFields: [],
        unmappedErrors: []
      };
    }

    const mappedFields: string[] = [];
    const unmappedErrors: string[] = [];

    for (const [rawPropertyPath, rawMessages] of entries) {
      const messages = Array.isArray(rawMessages) ? rawMessages : [String(rawMessages)];
      const firstMessage = messages.find(m => typeof m === 'string' && m.trim().length > 0) ?? '';
      const translatedMessage = this.translateFieldErrorMessage(firstMessage);

      const control = this.resolveControlByPath(form, rawPropertyPath);
      if (control) {
        const existingErrors = control.errors ? { ...control.errors } : {};
        control.setErrors({
          ...existingErrors,
          serverError: translatedMessage
        });
        control.markAsTouched();
        control.markAsDirty();

        // Automatically clear serverError when the user modifies the field
        control.valueChanges.pipe(take(1)).subscribe(() => {
          if (control.errors && 'serverError' in control.errors) {
            const { serverError: _, ...remainingErrors } = control.errors;
            const hasRemaining = Object.keys(remainingErrors).length > 0;
            control.setErrors(hasRemaining ? remainingErrors : null);
          }
        });

        mappedFields.push(rawPropertyPath);
      } else {
        unmappedErrors.push(translatedMessage);
      }
    }

    return {
      handledByFields: mappedFields.length > 0 && unmappedErrors.length === 0,
      mappedFields,
      unmappedErrors
    };
  }

  /**
   * Translates a backend field validation message into a localized user-friendly string
   * without using raw backend English messages as translation keys.
   */
  translateFieldErrorMessage(rawMessage: string): string {
    if (!rawMessage) {
      return this.i18n.translate('validation.invalidValue');
    }

    const lower = rawMessage.toLowerCase();

    if (lower.includes('not be empty') || lower.includes('is required') || lower.includes('must be provided')) {
      return this.i18n.translate('validation.required');
    }

    if (lower.includes('valid email') || lower.includes('email address')) {
      return this.i18n.translate('validation.email');
    }

    const minLengthMatch = /(?:at least|minimum length of)\s*(\d+)\s*characters/i.exec(rawMessage);
    if (minLengthMatch) {
      return this.i18n.translate('validation.minlength', { min: Number(minLengthMatch[1]) });
    }

    const maxLengthMatch = /(?:not exceed|maximum length of|must be|fewer than)\s*(\d+)\s*characters/i.exec(rawMessage);
    if (maxLengthMatch) {
      return this.i18n.translate('validation.maxlength', { max: Number(maxLengthMatch[1]) });
    }

    if (
      lower.includes('greater than 0') ||
      lower.includes("greater than '0'") ||
      lower.includes('greater than zero') ||
      lower.includes('must be positive')
    ) {
      return this.i18n.translate('validation.positiveNumber');
    }

    if (lower.includes('greater than or equal to 0') || lower.includes('cannot be negative') || lower.includes('not be negative')) {
      return this.i18n.translate('validation.nonNegativeNumber');
    }

    const minMatch = /greater than or equal to\s*(\d+(?:\.\d+)?)/i.exec(rawMessage);
    if (minMatch) {
      return this.i18n.translate('validation.min', { min: Number(minMatch[1]) });
    }

    const maxMatch = /less than or equal to\s*(\d+(?:\.\d+)?)/i.exec(rawMessage);
    if (maxMatch) {
      return this.i18n.translate('validation.max', { max: Number(maxMatch[1]) });
    }

    if (lower.includes('future')) {
      return this.i18n.translate('validation.futureDate');
    }

    if (lower.includes('at least one') && (lower.includes('line') || lower.includes('item'))) {
      return this.i18n.translate('validation.atLeastOneLine');
    }

    return this.i18n.translate('validation.invalidValue');
  }

  /**
   * Resolves a dot/bracket property path from .NET ValidationProblemDetails
   * (e.g. "CustomerId", "Lines[0].UnitPrice", "$.dueDate") to an Angular AbstractControl.
   */
  resolveControlByPath(rootGroup: FormGroup, rawPath: string): AbstractControl | null {
    if (!rawPath) return null;

    // Strip leading JSONPath '$.' if present
    const cleanedPath = rawPath.replace(/^\$\.?/, '').trim();
    if (!cleanedPath) return null;

    // Split by '.' and bracket indices '[0]'
    const segments = cleanedPath
      .replace(/\[(\d+)\]/g, '.$1')
      .split('.')
      .filter(Boolean);

    let current: AbstractControl | null = rootGroup;

    for (const segment of segments) {
      if (!current) return null;

      if (current instanceof FormArray) {
        const index = Number(segment);
        if (!Number.isInteger(index) || index < 0 || index >= current.length) {
          return null;
        }
        current = current.at(index);
        continue;
      }

      if (current instanceof FormGroup) {
        current = this.findControlInGroup(current, segment);
        continue;
      }

      return null;
    }

    return current;
  }

  private findControlInGroup(group: FormGroup, segment: string): AbstractControl | null {
    // 1. Exact match
    if (group.controls[segment]) {
      return group.controls[segment];
    }

    // 2. camelCase conversion (e.g., "CustomerId" -> "customerId")
    const camelSegment = segment.charAt(0).toLowerCase() + segment.slice(1);
    if (group.controls[camelSegment]) {
      return group.controls[camelSegment];
    }

    // 3. Known alias match (e.g., "ExpectedDelivery" -> "expectedDeliveryDate")
    const lowerSegment = segment.toLowerCase();
    const alias = FIELD_NAME_ALIASES[lowerSegment];
    if (alias && group.controls[alias]) {
      return group.controls[alias];
    }

    // 4. Case-insensitive fallback scan of group controls
    const controlKey = Object.keys(group.controls).find(
      key => key.toLowerCase() === lowerSegment
    );
    return controlKey ? group.controls[controlKey] : null;
  }

  private extractFieldErrors(
    errorOrNormalized: NormalizedApiError | { normalizedError?: NormalizedApiError; error?: unknown } | null | undefined
  ): Record<string, string[]> {
    if (!errorOrNormalized || typeof errorOrNormalized !== 'object') {
      return {};
    }

    if ('fieldErrors' in errorOrNormalized && errorOrNormalized.fieldErrors) {
      return errorOrNormalized.fieldErrors;
    }

    if ('normalizedError' in errorOrNormalized && errorOrNormalized.normalizedError?.fieldErrors) {
      return errorOrNormalized.normalizedError.fieldErrors;
    }

    if ('error' in errorOrNormalized && errorOrNormalized.error && typeof errorOrNormalized.error === 'object') {
      const payload = errorOrNormalized.error as Record<string, unknown>;
      if (payload['errors'] && typeof payload['errors'] === 'object') {
        const result: Record<string, string[]> = {};
        for (const [k, v] of Object.entries(payload['errors'] as Record<string, unknown>)) {
          if (Array.isArray(v)) {
            result[k] = v.map(String);
          } else if (typeof v === 'string') {
            result[k] = [v];
          }
        }
        return result;
      }
    }

    return {};
  }
}
