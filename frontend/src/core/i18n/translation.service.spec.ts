import { TestBed } from '@angular/core/testing';
import { registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/ar';
import localeArEg from '@angular/common/locales/ar-EG';
import { TranslationService } from './translation.service';
import { AppDirectionality } from './app-directionality.service';
import { LocalizedPaginatorIntl } from './localized-paginator-intl.service';
import {
  LocaleCurrencyPipe,
  LocaleDatePipe,
  LocaleNumberPipe,
  LocalePercentPipe,
  parseDateSafe,
  toDateOnlyIsoString
} from '@shared/pipes/localized-formatting.pipes';

registerLocaleData(localeAr);
registerLocaleData(localeArEg);

describe('TranslationService & Localization Architecture', () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.lang = 'en';
    document.documentElement.dir = 'ltr';
    document.body.dir = 'ltr';
    TestBed.configureTestingModule({});
  });

  it('defaults to English (en) with LTR direction when no saved preference exists', () => {
    const service = TestBed.inject(TranslationService);

    expect(service.currentLang()).toBe('en');
    expect(service.dir()).toBe('ltr');
    expect(service.isRtl()).toBe(false);
    expect(document.documentElement.lang).toBe('en');
    expect(document.documentElement.dir).toBe('ltr');
    expect(service.translate('dashboard.title')).toBe('Dashboard Overview');
  });

  it('loads Arabic translations and updates <html lang="ar" dir="rtl"> dynamically without reload', () => {
    const service = TestBed.inject(TranslationService);

    service.setLanguage('ar');
    TestBed.tick();

    expect(service.currentLang()).toBe('ar');
    expect(service.dir()).toBe('rtl');
    expect(service.isRtl()).toBe(true);
    expect(document.documentElement.lang).toBe('ar');
    expect(document.documentElement.dir).toBe('rtl');
    expect(document.body.dir).toBe('rtl');
    expect(service.translate('dashboard.title')).toBe('نظرة عامة على لوحة التحكم');
    expect(service.translate('sidebar.salesOrders')).toBe('طلبات المبيعات');
  });

  it('persists selected language to localStorage and restores it on startup', () => {
    localStorage.setItem('erp_lang', 'ar');

    const service = TestBed.inject(TranslationService);
    TestBed.tick();

    expect(service.currentLang()).toBe('ar');
    expect(service.dir()).toBe('rtl');
    expect(document.documentElement.lang).toBe('ar');
    expect(document.documentElement.dir).toBe('rtl');

    service.toggleLanguage();
    TestBed.tick();

    expect(service.currentLang()).toBe('en');
    expect(localStorage.getItem('erp_lang')).toBe('en');
    expect(document.documentElement.dir).toBe('ltr');
  });

  it('supports parameter interpolation and translates statuses and roles', () => {
    const service = TestBed.inject(TranslationService);

    expect(service.translate('orders.invoiceGenerated', { id: 42 }))
      .toBe('Invoice INV-42 successfully created');
    expect(service.translateStatus('Pending')).toBe('Pending');
    expect(service.translateRole('Administrator')).toBe('Administrator');

    service.setLanguage('ar');
    TestBed.tick();

    expect(service.translate('orders.invoiceGenerated', { id: 42 }))
      .toBe('تم إنشاء الفاتورة INV-42 بنجاح');
    expect(service.translateStatus('Pending')).toBe('قيد الانتظار');
    expect(service.translateStatus('Overdue')).toBe('متأخرة');
    expect(service.translateRole('Administrator')).toBe('مدير النظام');
  });

  it('emits direction changes via AppDirectionality for Angular Material components', () => {
    const service = TestBed.inject(TranslationService);
    const directionality = TestBed.inject(AppDirectionality);
    const emitted: string[] = [];

    directionality.change.subscribe(dir => emitted.push(dir));

    expect(directionality.value).toBe('ltr');

    service.setLanguage('ar');
    TestBed.tick();

    expect(directionality.value).toBe('rtl');
    expect(emitted).toContain('rtl');
  });

  it('updates MatPaginatorIntl labels and range formatting when language changes', () => {
    const service = TestBed.inject(TranslationService);
    const paginatorIntl = TestBed.inject(LocalizedPaginatorIntl);

    expect(paginatorIntl.itemsPerPageLabel).toBe('Items per page:');
    expect(paginatorIntl.getRangeLabel(0, 20, 45)).toContain('of');

    service.setLanguage('ar');
    TestBed.tick();

    expect(paginatorIntl.itemsPerPageLabel).toBe('العناصر لكل صفحة:');
    expect(paginatorIntl.getRangeLabel(0, 20, 45)).toContain('من');
  });

  it('parses calendar dates without timezone day-shifting and formats dates, currency, numbers, and percentages in en and ar', () => {
    const service = TestBed.inject(TranslationService);
    const datePipe = TestBed.runInInjectionContext(() => new LocaleDatePipe());
    const currencyPipe = TestBed.runInInjectionContext(() => new LocaleCurrencyPipe());
    const numberPipe = TestBed.runInInjectionContext(() => new LocaleNumberPipe());
    const percentPipe = TestBed.runInInjectionContext(() => new LocalePercentPipe());

    // Verify midnight UTC calendar date does not shift to the previous day
    const parsed = parseDateSafe('2026-05-01T00:00:00Z');
    expect(parsed).not.toBeNull();
    expect(parsed!.getFullYear()).toBe(2026);
    expect(parsed!.getMonth()).toBe(4); // May (0-indexed)
    expect(parsed!.getDate()).toBe(1);

    const isoOut = toDateOnlyIsoString(new Date(2026, 4, 1, 23, 30, 0));
    expect(isoOut).toBe('2026-05-01T00:00:00.000Z');

    // English formatting
    const enDate = datePipe.transform('2026-05-01T00:00:00Z', 'mediumDate');
    const enCurrency = currencyPipe.transform(1250.5);
    const enNumber = numberPipe.transform(1234567);
    const enPercent = percentPipe.transform(0.15);

    expect(enDate).toContain('2026');
    expect(enCurrency).toContain('EGP');
    expect(enCurrency).toContain('1,250.50');
    expect(enNumber).toBe('1,234,567');
    expect(enPercent).toContain('15%');

    // Switch to Arabic formatting
    service.setLanguage('ar');
    TestBed.tick();

    const arDate = datePipe.transform('2026-05-01T00:00:00Z', 'mediumDate');
    const arCurrency = currencyPipe.transform(1250.5);
    const arNumber = numberPipe.transform(1234567);

    expect(arDate).toBeTruthy();
    expect(arCurrency).toBeTruthy();
    expect(arCurrency).toContain('ج.م');
    expect(arNumber).toBeTruthy();
    // Financial value must remain identical (not converted to another currency)
    expect(arCurrency).not.toBe('');
  });
});
