import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="auth-layout-container">
      <div class="auth-layout-content">
        <router-outlet />
      </div>
    </div>
  `,
  styles: [`
    .auth-layout-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
      padding: 1.5rem 1rem;
    }
    .auth-layout-content {
      width: 100%;
      max-width: 460px;
      display: flex;
      justify-content: center;
    }
  `]
})
export class AuthLayoutComponent {}
