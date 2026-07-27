import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '@core/auth/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { catchError, finalize } from 'rxjs/operators';
import { EMPTY } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
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
            <mat-icon>person_add</mat-icon>
          </div>
          <mat-card-title>Create an Account</mat-card-title>
          <mat-card-subtitle>Join the ERP system</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="auth-form">
            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>First Name</mat-label>
                <input matInput formControlName="firstName" placeholder="John">
                @if (registerForm.controls.firstName.hasError('required')) {
                  <mat-error>First name is required</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Last Name</mat-label>
                <input matInput formControlName="lastName" placeholder="Doe">
                @if (registerForm.controls.lastName.hasError('required')) {
                  <mat-error>Last name is required</mat-error>
                }
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email address</mat-label>
              <input matInput type="email" formControlName="email" placeholder="name@company.com">
              <mat-icon matPrefix>email</mat-icon>
              @if (registerForm.controls.email.hasError('required')) {
                <mat-error>Email is required</mat-error>
              }
              @if (registerForm.controls.email.hasError('email')) {
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
              @if (registerForm.controls.password.hasError('required')) {
                <mat-error>Password is required</mat-error>
              }
              @if (registerForm.controls.password.hasError('minlength')) {
                <mat-error>Password must be at least 6 characters</mat-error>
              }
            </mat-form-field>

            <button mat-flat-button color="primary" type="submit" class="full-width submit-btn" [disabled]="registerForm.invalid || isLoading()">
              @if (isLoading()) {
                <mat-spinner diameter="20" class="btn-spinner"></mat-spinner>
                <span>Creating account...</span>
              } @else {
                <span>Register</span>
              }
            </button>
          </form>
        </mat-card-content>

        <mat-card-actions class="auth-footer">
          <span>Already have an account?</span>
          <a routerLink="/auth/login" color="primary">Sign in</a>
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

    .form-row {
      display: flex;
      gap: 16px;
    }

    @media (max-width: 480px) {
      .form-row {
        flex-direction: column;
        gap: 0;
      }
    }

    .half-width {
      flex: 1;
    }

    .full-width {
      width: 100%;
    }

    .submit-btn {
      height: 48px;
      font-size: 1rem;
      margin-top: 16px;
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
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notification = inject(NotificationService);

  readonly hidePassword = signal(true);
  readonly isLoading = signal(false);

  readonly registerForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading.set(true);
    
    this.authService.register(this.registerForm.getRawValue())
      .pipe(
        catchError(error => {
          const msg = error.error?.message || 'Registration failed. Please try again.';
          this.notification.error(msg);
          return EMPTY;
        }),
        finalize(() => this.isLoading.set(false))
      )
      .subscribe(() => {
        this.notification.success('Account created successfully');
        this.router.navigate(['/admin/dashboard']);
      });
  }
}
