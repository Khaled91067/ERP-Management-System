import { Component, signal, ViewChild, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatSidenavModule, MatSidenav } from '@angular/material/sidenav';
import { BreakpointObserver } from '@angular/cdk/layout';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, MatSidenavModule, HeaderComponent, SidebarComponent, FooterComponent],
  template: `
    <mat-sidenav-container class="layout-wrapper" [class.dark-theme-wrapper]="true" [hasBackdrop]="true">
      <mat-sidenav 
        #sidenav 
        mode="over" 
        [opened]="isMobile() && mobileSidenavOpen()" 
        class="mobile-sidenav"
        (closedStart)="onSidenavClosed()">
        @if (isMobile()) {
          <app-sidebar [collapsed]="false" (toggleCollapse)="toggleSidebar()" (navigationEnded)="onNavigationEnded()" />
        }
      </mat-sidenav>

      <mat-sidenav-content class="main-content-wrapper">
        <div class="layout-grid" [class.collapsed]="!isMobile() && sidebarCollapsed()" [class.mobile]="isMobile()">
          
          @if (!isMobile()) {
            <app-sidebar class="desktop-sidebar" [collapsed]="!isMobile() && sidebarCollapsed()" (toggleCollapse)="toggleSidebar()" (navigationEnded)="onNavigationEnded()" />
          }

          <div class="layout-main">
            <app-header (toggleSidebar)="toggleSidebar()" />
            <main class="content-area">
              <router-outlet />
            </main>
            <app-footer />
          </div>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .layout-wrapper {
      height: 100vh;
      width: 100vw;
      background-color: var(--bg-main);
    }
    
    .main-content-wrapper {
      display: flex;
      flex-direction: column;
      height: 100%;
      overflow: hidden; 
    }
    
    .layout-grid {
      display: grid;
      height: 100%;
      width: 100%;
      
      --sidebar-expanded-width: 240px;
      --sidebar-collapsed-width: 64px;
      
      grid-template-columns: var(--sidebar-expanded-width) 1fr;
      transition: grid-template-columns 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .layout-grid.collapsed {
      grid-template-columns: var(--sidebar-collapsed-width) 1fr;
    }
    
    .layout-grid.mobile {
      grid-template-columns: 1fr;
    }
    
    .desktop-sidebar {
      display: block;
      height: 100%;
      width: 100%;
      overflow: hidden;
      border-right: 1px solid var(--sidebar-border);
    }

    .mobile-sidenav {
      width: 280px;
    }
    
    .layout-main {
      display: flex;
      flex-direction: column;
      height: 100%;
      width: 100%;
      overflow: hidden;
    }
    
    .content-area {
      flex: 1;
      overflow-y: auto;
      padding: 1.5rem;
      width: 100%;
      box-sizing: border-box;
    }
    
    @media (max-width: 767px) {
      .content-area {
        padding: 1rem;
      }
    }
  `]
})
export class AdminLayoutComponent implements OnInit {
  private readonly breakpointObserver = inject(BreakpointObserver);
  
  readonly sidebarCollapsed = signal<boolean>(false);
  readonly isMobile = signal<boolean>(false);
  readonly isTablet = signal<boolean>(false);
  readonly mobileSidenavOpen = signal<boolean>(false);

  @ViewChild('sidenav') sidenav!: MatSidenav;

  ngOnInit() {
    this.breakpointObserver.observe([
      '(max-width: 767.98px)',
      '(min-width: 768px) and (max-width: 1279.98px)',
      '(min-width: 1280px)'
    ]).subscribe(() => {
      const mobile = this.breakpointObserver.isMatched('(max-width: 767.98px)');
      const tablet = this.breakpointObserver.isMatched('(min-width: 768px) and (max-width: 1279.98px)');

      this.isMobile.set(mobile);
      this.isTablet.set(tablet);

      if (mobile) {
         this.mobileSidenavOpen.set(false);
      } else if (tablet) {
         this.sidebarCollapsed.set(true);
         this.mobileSidenavOpen.set(false);
      } else {
         this.sidebarCollapsed.set(false);
         this.mobileSidenavOpen.set(false);
      }
    });
  }

  toggleSidebar(): void {
    if (this.isMobile()) {
      this.mobileSidenavOpen.update(val => !val);
      if (this.mobileSidenavOpen()) {
        this.sidenav.open();
      } else {
        this.sidenav.close();
      }
    } else {
      this.sidebarCollapsed.update(val => !val);
    }
  }

  onSidenavClosed(): void {
    this.mobileSidenavOpen.set(false);
  }
  
  onNavigationEnded(): void {
    if (this.isMobile()) {
      this.sidenav.close();
      this.mobileSidenavOpen.set(false);
    }
  }
}
