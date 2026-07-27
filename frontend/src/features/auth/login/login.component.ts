import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '@core/auth/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { catchError, finalize } from 'rxjs/operators';
import { EMPTY } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="auth-wrapper">
      <mat-card class="auth-card">
        <mat-card-header class="auth-header">
          <div mat-card-avatar class="auth-avatar">
            <mat-icon>business</mat-icon>
          </div>
          <mat-card-title>Welcome Back</mat-card-title>
          <mat-card-subtitle>Sign in to your ERP account</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="auth-form">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email address</mat-label>
              <input matInput type="email" formControlName="email" placeholder="name@company.com">
              <mat-icon matPrefix>email</mat-icon>
              @if (loginForm.controls.email.hasError('required')) {
                <mat-error>Email is required</mat-error>
              }
              @if (loginForm.controls.email.hasError('email')) {
                <mat-error>Please enter a valid email address</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword() ? 'password' : 'text'" formControlName="password">
              <mat-icon matPrefix>lock</mat-icon>
              <button mat-icon-button matSuffix (click)="hidePassword.set(!hidePassword())" type="button" 
                      [attr.aria-label]="hidePassword() ? 'Show password' : 'Hide password'" [attr.aria-pressed]="!hidePassword()">
                <mat-icon>{{hidePassword() ? 'visibility_off' : 'visibility'}}</mat-icon>
              </button>
              @if (loginForm.controls.password.hasError('required')) {
                <mat-error>Password is required</mat-error>
              }
            </mat-form-field>

            <div class="auth-actions-row">
              <mat-checkbox formControlName="rememberMe">Remember me</mat-checkbox>
              <a href="javascript:void(0)" class="forgot-link">Forgot password?</a>
            </div>

            <button mat-flat-button color="primary" type="submit" class="full-width submit-btn" [disabled]="loginForm.invalid || isLoading()">
              @if (isLoading()) {
                <mat-spinner diameter="20" class="btn-spinner"></mat-spinner>
                <span>Signing in...</span>
              } @else {
                <span>Sign in</span>
              }
            </button>
          </form>
        </mat-card-content>

        <mat-card-actions class="auth-footer">
          <span>Don't have an account?</span>
          <a routerLink="/auth/register" color="primary">Register here</a>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .auth-wrapper {
      width: 100%;
      display: flex;
      justify-content: center;
    }

    .auth-card {
      width: 100%;
      max-width: 440px;
      padding: 32px 24px;
      background-color: var(--surface-card, #ffffff);
      border-radius: 12px;
    }

    .auth-header {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      margin-bottom: 24px;
    }

    .auth-avatar {
      background-color: rgba(37, 99, 235, 0.1);
      color: var(--primary-color);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 16px;
      width: 56px;
      height: 56px;
      border-radius: 50%;
    }
    
    .auth-avatar mat-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
    }

    mat-card-title {
      font-size: 1.5rem !important;
      font-weight: 700 !important;
      margin-bottom: 8px !important;
      color: var(--text-primary);
    }

    mat-card-subtitle {
      font-size: 0.9375rem !important;
      color: var(--text-secondary);
    }

    .auth-form {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .full-width {
      width: 100%;
    }

    .auth-actions-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      margin-top: -4px;
    }

    .forgot-link {
      color: var(--primary-color);
      text-decoration: none;
      font-size: 0.875rem;
      font-weight: 500;
    }
    
    .forgot-link:hover {
      text-decoration: underline;
    }

    .submit-btn {
      height: 48px;
      font-size: 1rem;
    }

    .btn-spinner {
      display: inline-block;
      margin-right: 8px;
    }

    .auth-footer {
      display: flex;
      justify-content: center;
      gap: 8px;
      padding-top: 24px !important;
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .auth-footer a {
      color: var(--primary-color);
      text-decoration: none;
      font-weight: 500;
    }
    
    .auth-footer a:hover {
      text-decoration: underline;
    }
  `]
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notification = inject(NotificationService);

  readonly hidePassword = signal(true);
  readonly isLoading = signal(false);

  readonly loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [false]
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading.set(true);
    const { email, password } = this.loginForm.getRawValue();

    this.authService.login({ email, password })
      .pipe(
        catchError(error => {
          const msg = error.error?.message || 'Invalid email or password';
          this.notification.error(msg);
          return EMPTY;
        }),
        finalize(() => this.isLoading.set(false))
      )
      .subscribe(() => {
        this.notification.success('Successfully logged in');
        this.router.navigate(['/admin/dashboard']);
      });
  }
}
