import { InjectionToken } from '@angular/core';

export interface AppConfig {
  apiUrl: string;
  appName: string;
  production: boolean;
  defaultLanguage: string;
}

export const APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG');
