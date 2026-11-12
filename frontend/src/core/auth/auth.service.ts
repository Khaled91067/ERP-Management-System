import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { User, UserRole } from '../models/user.model';
import { AuthResponse, AuthState } from '../models/auth.model';
import { ApiService } from '../services/api.service';
import { withErrorHandlingOptions } from '../http/http-context.tokens';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly router = inject(Router);
  private readonly apiService = inject(ApiService);

  private readonly TOKEN_KEY = 'erp_auth_token';
  private readonly REFRESH_TOKEN_KEY = 'erp_refresh_token';

  // Signals for state
  private readonly state = signal<AuthState>({
    user: this.loadSavedUser(),
    token: localStorage.getItem(this.TOKEN_KEY),
    refreshToken: localStorage.getItem(this.REFRESH_TOKEN_KEY),
    isAuthenticated: !!localStorage.getItem(this.TOKEN_KEY)
  });

  // Selectors
  readonly currentUser = computed(() => this.state().user);
  readonly token = computed(() => this.state().token);
  readonly refreshToken = computed(() => this.state().refreshToken ?? null);
  readonly isAuthenticated = computed(() => this.state().isAuthenticated);
  readonly userRole = computed(() => this.state().user?.role ?? null);
  readonly userPermissions = computed(() => this.state().user?.permissions ?? []);

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
      const decoded = JSON.parse(jsonPayload) as Record<string, unknown>;

      const email = String(
        decoded['email']
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
        || emailFallback
        || ''
      );

      const rawRole = decoded['role']
        || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        || '';
      const role = typeof rawRole === 'string' ? rawRole : (Array.isArray(rawRole) ? String(rawRole[0] ?? '') : '');

      const id = String(
        decoded['sub']
        || decoded['nameid']
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
        || ''
      );

      const firstName = String(
        decoded['given_name']
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname']
        || (email ? email.split('@')[0] : 'User')
      );
      const lastName = String(
        decoded['family_name']
        || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname']
        || ''
      );

      const rawPerms = decoded['permission'] || decoded['permissions'] || [];
      const permissions: string[] = Array.isArray(rawPerms)
        ? rawPerms.map(String)
        : (typeof rawPerms === 'string' && rawPerms ? [rawPerms] : []);

      return {
        id,
        email,
        firstName,
        lastName,
        role: role as UserRole,
        permissions,
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
            this.setSession(user, token, response.refreshToken);
          }
        })
      );
  }

  refreshAccessToken(): Observable<AuthResponse> {
    const currentRefreshToken = this.refreshToken() || localStorage.getItem(this.REFRESH_TOKEN_KEY);
    return this.apiService.create<{ refreshToken: string }, AuthResponse>(
      'auth/refresh',
      { refreshToken: currentRefreshToken ?? '' },
      withErrorHandlingOptions({ skipGlobalNotification: true })
    ).pipe(
      tap(response => {
        const token = response.accessToken || response.token || '';
        if (!token) return;
        const user = response.user || this.extractUserFromToken(token, this.currentUser()?.email ?? '');
        if (user) {
          this.setSession(user, token, response.refreshToken);
        }
      })
    );
  }

  setSession(user: User, token: string, refreshToken?: string | null): void {
    if (token) {
      localStorage.setItem(this.TOKEN_KEY, token);
    }
    if (refreshToken) {
      localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
    }
    if (user) {
      localStorage.setItem('erp_user', JSON.stringify(user));
    }
    this.state.set({
      user,
      token,
      refreshToken: refreshToken ?? this.state().refreshToken ?? null,
      isAuthenticated: !!token && !!user
    });
  }

  logout(): void {
    const currentRefreshToken = this.refreshToken() || localStorage.getItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem('erp_user');
    this.state.set({
      user: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false
    });

    if (currentRefreshToken) {
      this.apiService.create<{ refreshToken: string }, void>(
        'auth/logout',
        { refreshToken: currentRefreshToken },
        withErrorHandlingOptions({ skipGlobalNotification: true })
      )
        .pipe(catchError(() => of(undefined)))
        .subscribe();
    }

    this.router.navigate(['/auth/login']);
  }

  hasRole(role: string): boolean {
    const user = this.currentUser();
    return user ? user.role === role : false;
  }

  hasPermission(permission: string): boolean {
    const user = this.currentUser();
    if (!user) return false;
    if (user.role === 'Administrator') return true;
    return user.permissions?.includes(permission) ?? false;
  }

  hasAnyPermission(permissions: string[]): boolean {
    const user = this.currentUser();
    if (!user) return false;
    if (user.role === 'Administrator') return true;
    return permissions.some(p => user.permissions?.includes(p));
  }
}
