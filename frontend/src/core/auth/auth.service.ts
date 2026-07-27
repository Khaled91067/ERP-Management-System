import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { User, AuthState } from '../models/user.model';
import { APP_CONFIG } from '../config/app-config.token';
import { ApiService } from '../services/api.service';

export interface AuthResponse {
  accessToken?: string;
  token?: string;
  refreshToken?: string;
  user?: User;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly router = inject(Router);
  private readonly apiService = inject(ApiService);

  private readonly TOKEN_KEY = 'erp_auth_token';

  // Signals for state
  private readonly state = signal<AuthState>({
    user: this.loadSavedUser(),
    token: localStorage.getItem(this.TOKEN_KEY),
    isAuthenticated: !!localStorage.getItem(this.TOKEN_KEY)
  });

  // Selectors
  readonly currentUser = computed(() => this.state().user);
  readonly token = computed(() => this.state().token);
  readonly isAuthenticated = computed(() => this.state().isAuthenticated);
  readonly userRole = computed(() => this.state().user?.role ?? null);

  private loadSavedUser(): User | null {
    const saved = localStorage.getItem('erp_user');
    if (!saved) return null;
    try {
      return JSON.parse(saved) as User;
    } catch {
      return null;
    }
  }

  private extractUserFromToken(token: string, emailFallback: string): User {
    try {
      const payloadBase64 = token.split('.')[1];
      if (payloadBase64) {
        const decoded = JSON.parse(atob(payloadBase64));
        const email = decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || emailFallback;
        const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'ADMIN';
        return {
          id: decoded.sub || '1',
          email,
          firstName: email.split('@')[0] || 'User',
          lastName: '',
          role: role as any,
          permissions: [],
          isActive: true
        };
      }
    } catch {
      // fallback
    }
    return {
      id: '1',
      email: emailFallback || 'admin@erp.com',
      firstName: 'Admin',
      lastName: 'User',
      role: 'ADMIN',
      permissions: [],
      isActive: true
    };
  }

  login(credentials: Record<string, string>): Observable<AuthResponse> {
    return this.apiService.create<Record<string, string>, AuthResponse>('auth/login', credentials)
      .pipe(
        tap(response => {
          const token = response.accessToken || response.token || '';
          const user = response.user || this.extractUserFromToken(token, credentials['email']);
          this.setSession(user, token);
        })
      );
  }

  register(userData: Record<string, string>): Observable<any> {
    return this.apiService.create<Record<string, string>, any>('auth/register', userData)
      .pipe(
        tap(response => {
          const token = response.accessToken || response.token || '';
          const user = response.user || this.extractUserFromToken(token, userData['email']);
          this.setSession(user, token);
        })
      );
  }

  setSession(user: User, token: string): void {
    if (token) {
      localStorage.setItem(this.TOKEN_KEY, token);
    }
    if (user) {
      localStorage.setItem('erp_user', JSON.stringify(user));
    }
    this.state.set({
      user,
      token,
      isAuthenticated: !!token
    });
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem('erp_user');
    this.state.set({
      user: null,
      token: null,
      isAuthenticated: false
    });
    this.router.navigate(['/auth/login']);
  }

  hasRole(role: string): boolean {
    const user = this.currentUser();
    return user ? user.role === role : false;
  }
}
