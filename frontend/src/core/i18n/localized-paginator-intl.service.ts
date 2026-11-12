import { Injectable, effect, inject } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslationService } from './translation.service';

@Injectable({
  providedIn: 'root'
})
export class LocalizedPaginatorIntl extends MatPaginatorIntl {
  private readonly i18n = inject(TranslationService);

  constructor() {
    super();
    effect(() => {
      this.i18n.currentLang();
      this.updateLabels();
    });
  }

  private updateLabels(): void {
    this.itemsPerPageLabel = this.i18n.translate('common.pagination.itemsPerPage');
    this.nextPageLabel = this.i18n.translate('common.pagination.nextPage');
    this.previousPageLabel = this.i18n.translate('common.pagination.previousPage');
    this.firstPageLabel = this.i18n.translate('common.pagination.firstPage');
    this.lastPageLabel = this.i18n.translate('common.pagination.lastPage');
    this.changes.next();
  }

  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
    const locale = this.i18n.localeId();
    const formatter = new Intl.NumberFormat(locale);

    if (length === 0 || pageSize === 0) {
      return this.i18n.translate('common.pagination.empty', {
        length: formatter.format(length)
      });
    }

    const normalizedLength = Math.max(length, 0);
    const startIndex = page * pageSize;
    const endIndex =
      startIndex < normalizedLength
        ? Math.min(startIndex + pageSize, normalizedLength)
        : startIndex + pageSize;

    const ofLabel = this.i18n.translate('common.pagination.of');
    return `${formatter.format(startIndex + 1)} – ${formatter.format(endIndex)} ${ofLabel} ${formatter.format(normalizedLength)}`;
  };
}
