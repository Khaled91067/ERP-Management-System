import { Component, inject } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';

@Component({
  selector: 'app-footer',
  standalone: true,
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  readonly config = inject(APP_CONFIG);
  readonly currentYear = new Date().getFullYear();
}
