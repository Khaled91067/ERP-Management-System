import { Pipe, PipeTransform, inject } from '@angular/core';
import { TranslationService } from '@core/i18n/translation.service';

/**
 * Safely parses date values while preventing accidental day shifts caused by
 * UTC-to-local timezone conversions on date-only or midnight ISO strings.
 */
export function parseDateSafe(value: Date | string | number | null | undefined): Date | null {
  if (value === null || value === undefined || value === '') {
    return null;
  }

  if (value instanceof Date) {
    return isNaN(value.getTime()) ? null : value;
  }

  if (typeof value === 'number') {
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d;
  }

  const trimmed = value.trim();
  // Match date-only YYYY-MM-DD or midnight ISO YYYY-MM-DDT00:00:00(.000)(Z)
  const dateOnlyMatch = /^(\d{4})-(\d{2})-(\d{2})(?:T00:00:00(?:\.0+)?Z?)?$/.exec(trimmed);
  if (dateOnlyMatch) {
    const year = Number(dateOnlyMatch[1]);
    const month = Number(dateOnlyMatch[2]) - 1;
    const day = Number(dateOnlyMatch[3]);
    // Construct at local noon (12:00) so timezone offsets cannot shift the calendar day
    return new Date(year, month, day, 12, 0, 0);
  }

  const parsed = new Date(trimmed);
  return isNaN(parsed.getTime()) ? null : parsed;
}

/**
 * Serializes a Date selected from a datepicker into an ISO 8601 UTC string
 * that preserves the user's selected calendar year, month, and day regardless of local timezone offset.
 */
export function toDateOnlyIsoString(value: Date | string | null | undefined): string {
  const parsed = parseDateSafe(value);
  if (!parsed) {
    return new Date().toISOString();
  }
  const utcDate = new Date(Date.UTC(parsed.getFullYear(), parsed.getMonth(), parsed.getDate(), 0, 0, 0, 0));
  return utcDate.toISOString();
}

export type DateFormatPreset = 'shortDate' | 'mediumDate' | 'longDate' | 'fullDate' | 'short' | 'medium';

@Pipe({
  name: 'localDate',
  standalone: true,
  pure: false
})
export class LocaleDatePipe implements PipeTransform {
  private readonly i18n = inject(TranslationService);

  transform(
    value: Date | string | number | null | undefined,
    format: DateFormatPreset = 'mediumDate'
  ): string {
    const date = parseDateSafe(value);
    if (!date) return '';

    const locale = this.i18n.localeId();
    const options = this.getFormatOptions(format);

    try {
      return new Intl.DateTimeFormat(locale, options).format(date);
    } catch {
      return date.toLocaleDateString();
    }
  }

  private getFormatOptions(format: DateFormatPreset): Intl.DateTimeFormatOptions {
    switch (format) {
      case 'shortDate':
        return { year: 'numeric', month: 'numeric', day: 'numeric' };
      case 'longDate':
        return { year: 'numeric', month: 'long', day: 'numeric' };
      case 'fullDate':
        return { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
      case 'short':
        return { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: '2-digit' };
      case 'medium':
        return { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' };
      case 'mediumDate':
      default:
        return { year: 'numeric', month: 'short', day: 'numeric' };
    }
  }
}

export const DEFAULT_CURRENCY_CODE = 'EGP';

@Pipe({
  name: 'localCurrency',
  standalone: true,
  pure: false
})
export class LocaleCurrencyPipe implements PipeTransform {
  private readonly i18n = inject(TranslationService);

  transform(
    value: number | string | null | undefined,
    currencyCode: string = DEFAULT_CURRENCY_CODE,
    minimumFractionDigits: number = 2,
    maximumFractionDigits: number = 2
  ): string {
    if (value === null || value === undefined || value === '') {
      return '';
    }

    const numericValue = typeof value === 'string' ? Number(value) : value;
    if (isNaN(numericValue)) {
      return '';
    }

    const locale = this.i18n.localeId();
    try {
      return new Intl.NumberFormat(locale, {
        style: 'currency',
        currency: currencyCode,
        minimumFractionDigits,
        maximumFractionDigits
      }).format(numericValue);
    } catch {
      return `${currencyCode} ${numericValue.toFixed(minimumFractionDigits)}`;
    }
  }
}

@Pipe({
  name: 'localNumber',
  standalone: true,
  pure: false
})
export class LocaleNumberPipe implements PipeTransform {
  private readonly i18n = inject(TranslationService);

  transform(
    value: number | string | null | undefined,
    minimumFractionDigits: number = 0,
    maximumFractionDigits: number = 2
  ): string {
    if (value === null || value === undefined || value === '') {
      return '';
    }

    const numericValue = typeof value === 'string' ? Number(value) : value;
    if (isNaN(numericValue)) {
      return '';
    }

    const locale = this.i18n.localeId();
    try {
      return new Intl.NumberFormat(locale, {
        minimumFractionDigits,
        maximumFractionDigits
      }).format(numericValue);
    } catch {
      return String(numericValue);
    }
  }
}

@Pipe({
  name: 'localPercent',
  standalone: true,
  pure: false
})
export class LocalePercentPipe implements PipeTransform {
  private readonly i18n = inject(TranslationService);

  transform(
    value: number | string | null | undefined,
    isWholePercentage: boolean = false,
    minimumFractionDigits: number = 0,
    maximumFractionDigits: number = 2
  ): string {
    if (value === null || value === undefined || value === '') {
      return '';
    }

    const numericValue = typeof value === 'string' ? Number(value) : value;
    if (isNaN(numericValue)) {
      return '';
    }

    const ratio = isWholePercentage ? numericValue / 100 : numericValue;
    const locale = this.i18n.localeId();

    try {
      return new Intl.NumberFormat(locale, {
        style: 'percent',
        minimumFractionDigits,
        maximumFractionDigits
      }).format(ratio);
    } catch {
      return `${(ratio * 100).toFixed(maximumFractionDigits)}%`;
    }
  }
}
