import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { User } from '../models/user.model';
import { AuthResponse, AuthState } from '../models/auth.model';
import { ApiService } from '../services/api.service';

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

  private extractUserFromToken(token: string, emailFallback: string = ''): User | null {
    if (!token) return null;
    try {
      const parts = token.split('.');
      if (parts.length < 2) return null;

      let base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      while (base64.length % 4 !== 0) {
        base64 += '=';
      }

      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      const decoded = JSON.parse(jsonPayload);

      const email = decoded.email 
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] 
        || emailFallback 
        || '';

      const rawRole = decoded.role 
        || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] 
        || 'VIEWER';
      const role = typeof rawRole === 'string' ? rawRole : (Array.isArray(rawRole) ? rawRole[0] : 'VIEWER');

      const id = decoded.sub 
        || decoded.nameid 
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] 
        || '';

      const firstName = decoded.given_name 
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname'] 
        || (email ? email.split('@')[0] : 'User');
      const lastName = decoded.family_name 
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname'] 
        || '';

      return {
        id,
        email,
        firstName,
        lastName,
        role: role as any,
        permissions: decoded.permissions ? (Array.isArray(decoded.permissions) ? decoded.permissions : [decoded.permissions]) : [],
        isActive: true
      };
    } catch {
      return null;
    }
  }

  login(credentials: Record<string, string>): Observable<AuthResponse> {
    return this.apiService.create<Record<string, string>, AuthResponse>('auth/login', credentials)
      .pipe(
        tap(response => {
          const token = response.accessToken || response.token || '';
          if (!token) return;
          const user = response.user || this.extractUserFromToken(token, credentials['email']);
          if (user) {
            this.setSession(user, token);
          }
        })
      );
  }

  register(userData: Record<string, string>): Observable<any> {
    return this.apiService.create<Record<string, string>, any>('auth/register', userData)
      .pipe(
        tap(response => {
          const token = response?.accessToken || response?.token;
          if (token) {
            const user = response?.user || this.extractUserFromToken(token, userData['email']);
            if (user) {
              this.setSession(user, token);
            }
          }
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
      isAuthenticated: !!token && !!user
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
