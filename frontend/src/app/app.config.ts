import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/ar';
import localeArEg from '@angular/common/locales/ar-EG';
import { provideRouter, withComponentInputBinding, withViewTransitions } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { Directionality } from '@angular/cdk/bidi';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { routes } from './app.routes';
import { authInterceptor } from '@core/interceptors/auth.interceptor';
import { errorInterceptor } from '@core/interceptors/error.interceptor';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppDirectionality } from '@core/i18n/app-directionality.service';
import { LocalizedPaginatorIntl } from '@core/i18n/localized-paginator-intl.service';
import { environment } from '@env/environment';

registerLocaleData(localeAr);
registerLocaleData(localeArEg);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideAnimationsAsync(),
    provideRouter(routes, withComponentInputBinding(), withViewTransitions()),
    provideHttpClient(
      withInterceptors([errorInterceptor, authInterceptor])
    ),
    {
      provide: APP_CONFIG,
      useValue: environment
    },
    {
      provide: Directionality,
      useExisting: AppDirectionality
    },
    {
      provide: MatPaginatorIntl,
      useClass: LocalizedPaginatorIntl
    }
  ]
};
