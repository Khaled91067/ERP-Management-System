import { Component, signal, ViewChild, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatSidenavModule, MatSidenav } from '@angular/material/sidenav';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, MatSidenavModule, HeaderComponent, SidebarComponent, FooterComponent],
  template: `
    <div class="layout-wrapper" [class.dark-theme-wrapper]="true">
      <mat-sidenav-container class="sidenav-container" [class.collapsed-container]="!isMobile() && sidebarCollapsed()" [hasBackdrop]="isMobile()">
        <mat-sidenav 
          #sidenav 
          [mode]="isMobile() ? 'over' : 'side'" 
          [opened]="!isMobile() || mobileSidenavOpen()" 
          class="sidenav"
          [class.collapsed]="!isMobile() && sidebarCollapsed()"
          (closedStart)="onSidenavClosed()">
          <app-sidebar [collapsed]="!isMobile() && sidebarCollapsed()" (toggleCollapse)="toggleSidebar()" (navigationEnded)="onNavigationEnded()" />
        </mat-sidenav>

        <mat-sidenav-content class="sidenav-content">
          <div class="layout-main">
            <app-header (toggleSidebar)="toggleSidebar()" />
            <main class="content-area">
              <router-outlet />
            </main>
            <app-footer />
          </div>
        </mat-sidenav-content>
      </mat-sidenav-container>
    </div>
  `,
  styles: [`
    .layout-wrapper {
      height: 100vh;
      width: 100vw;
      overflow: hidden;
    }
    
    .sidenav-container {
      height: 100%;
      background-color: var(--bg-main);
    }
    
    .sidenav {
      width: 240px !important;
      transition: width 0.3s cubic-bezier(0.4, 0, 0.2, 1) !important;
      background-color: var(--sidebar-bg);
      border-right: none;
      overflow-x: hidden;
    }
    
    .sidenav.collapsed {
      width: 64px !important;
    }

    ::ng-deep .mat-drawer-inner-container {
      overflow-x: hidden !important;
    }

    .sidenav-content {
      display: flex;
      flex-direction: column;
      background-color: var(--bg-main);
      /* Sidenav content automatically pushes when mode="side", no margin needed */
    }
    
    .layout-main {
      display: flex;
      flex-direction: column;
      height: 100%;
      width: 100%;
    }
    
    .content-area {
      flex: 1;
      overflow-y: auto;
      padding: 1.5rem;
      width: 100%;
      box-sizing: border-box;
    }
    
    @media (max-width: 600px) {
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
  readonly mobileSidenavOpen = signal<boolean>(false);

  @ViewChild('sidenav') sidenav!: MatSidenav;

  ngOnInit() {
    this.breakpointObserver.observe([
      Breakpoints.XSmall,
      Breakpoints.Small,
      Breakpoints.Medium
    ]).subscribe(result => {
      this.isMobile.set(result.matches);
      if (!result.matches && this.mobileSidenavOpen()) {
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
