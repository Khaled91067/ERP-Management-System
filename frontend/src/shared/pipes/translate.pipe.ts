import { Pipe, PipeTransform, inject } from '@angular/core';
import { TranslationService } from '@core/i18n/translation.service';

@Pipe({
  name: 'translate',
  standalone: true,
  pure: false
})
export class TranslatePipe implements PipeTransform {
  private readonly i18n = inject(TranslationService);

  transform(key: string | null | undefined, params?: Record<string, string | number>): string {
    if (!key) return '';
    // Read signal to subscribe the template view to language changes
    this.i18n.currentLang();
    return this.i18n.translate(key, params);
  }
}
