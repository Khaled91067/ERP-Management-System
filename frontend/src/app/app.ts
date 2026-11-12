import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslationService } from '@core/i18n/translation.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html'
})
export class App {
  // Eagerly initialize TranslationService so document lang and dir are restored on startup
  readonly i18n = inject(TranslationService);
}
