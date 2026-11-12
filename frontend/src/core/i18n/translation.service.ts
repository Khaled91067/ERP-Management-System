import { Injectable, inject, signal, computed, DOCUMENT } from '@angular/core';
import { DateAdapter } from '@angular/material/core';
import { APP_CONFIG } from '@core/config/app-config.token';
import enTranslations from '../../assets/i18n/en.json';
import arTranslations from '../../assets/i18n/ar.json';

export type SupportedLanguage = 'en' | 'ar';
export type LanguageDirection = 'ltr' | 'rtl';
export type TranslationDictionary = Record<string, unknown>;

export const LANGUAGE_STORAGE_KEY = 'erp_lang';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private readonly document = inject(DOCUMENT);
  private readonly config = inject(APP_CONFIG, { optional: true });
  private readonly dateAdapter = inject(DateAdapter, { optional: true });

  private readonly dictionaries = signal<Record<SupportedLanguage, TranslationDictionary>>({
    en: enTranslations as TranslationDictionary,
    ar: arTranslations as TranslationDictionary
  });

  readonly currentLang = signal<SupportedLanguage>(this.resolveInitialLanguage());
  readonly dir = computed<LanguageDirection>(() => (this.currentLang() === 'ar' ? 'rtl' : 'ltr'));
  readonly isRtl = computed<boolean>(() => this.dir() === 'rtl');
  readonly localeId = computed<string>(() => (this.currentLang() === 'ar' ? 'ar-EG' : 'en-EG'));

  constructor() {
    this.applyLanguageToDocument(this.currentLang());
  }

  private resolveInitialLanguage(): SupportedLanguage {
    try {
      const saved = localStorage.getItem(LANGUAGE_STORAGE_KEY);
      if (saved === 'en' || saved === 'ar') {
        return saved;
      }
    } catch {
      // Ignore storage access errors in restricted environments
    }

    const configuredDefault = this.config?.defaultLanguage;
    if (configuredDefault === 'ar' || configuredDefault === 'en') {
      return configuredDefault;
    }

    return 'en';
  }

  setLanguage(lang: SupportedLanguage): void {
    if (lang !== 'en' && lang !== 'ar') {
      return;
    }

    this.currentLang.set(lang);
    try {
      localStorage.setItem(LANGUAGE_STORAGE_KEY, lang);
    } catch {
      // Ignore storage errors
    }

    this.applyLanguageToDocument(lang);
  }

  toggleLanguage(): void {
    const next: SupportedLanguage = this.currentLang() === 'en' ? 'ar' : 'en';
    this.setLanguage(next);
  }

  setTranslations(lang: SupportedLanguage, dictionary: TranslationDictionary): void {
    this.dictionaries.update(current => ({
      ...current,
      [lang]: dictionary
    }));
  }

  translate(key: string, params?: Record<string, string | number>): string {
    if (!key) return '';

    const lang = this.currentLang();
    const allDictionaries = this.dictionaries();
    const activeDict = allDictionaries[lang];
    const fallbackDict = allDictionaries.en;

    let rawValue = this.resolvePath(activeDict, key);
    if (rawValue === undefined && lang !== 'en') {
      rawValue = this.resolvePath(fallbackDict, key);
    }

    if (typeof rawValue !== 'string') {
      return key;
    }

    if (!params) {
      return rawValue;
    }

    return rawValue.replace(/\{\{\s*(\w+)\s*\}\}/g, (_, paramKey: string) => {
      const val = params[paramKey];
      return val !== undefined && val !== null ? String(val) : '';
    });
  }

  t(key: string, params?: Record<string, string | number>): string {
    return this.translate(key, params);
  }

  translateStatus(status: string | null | undefined): string {
    if (!status) return '';
    const normalized = status.trim().toLowerCase();
    const key = `status.${normalized}`;
    const translated = this.translate(key);
    return translated === key ? status : translated;
  }

  translateRole(role: string | null | undefined): string {
    if (!role) return '';
    const trimmed = role.trim();
    const key = `roleNames.${trimmed}`;
    const translated = this.translate(key);
    return translated === key ? trimmed : translated;
  }

  private resolvePath(obj: TranslationDictionary | undefined, path: string): unknown {
    if (!obj) return undefined;
    if (Object.prototype.hasOwnProperty.call(obj, path)) {
      return obj[path];
    }

    const segments = path.split('.');
    let current: unknown = obj;

    for (const segment of segments) {
      if (current && typeof current === 'object' && Object.prototype.hasOwnProperty.call(current, segment)) {
        current = (current as Record<string, unknown>)[segment];
      } else {
        return undefined;
      }
    }

    return current;
  }

  private applyLanguageToDocument(lang: SupportedLanguage): void {
    const dir: LanguageDirection = lang === 'ar' ? 'rtl' : 'ltr';
    const htmlElement = this.document?.documentElement;
    if (htmlElement) {
      htmlElement.setAttribute('lang', lang);
      htmlElement.setAttribute('dir', dir);
    }
    const bodyElement = this.document?.body;
    if (bodyElement) {
      bodyElement.setAttribute('dir', dir);
    }

    if (this.dateAdapter) {
      this.dateAdapter.setLocale(lang === 'ar' ? 'ar-EG' : 'en-US');
    }
  }
}
