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
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent implements OnInit {
  private readonly breakpointObserver = inject(BreakpointObserver);
  
  readonly sidebarCollapsed = signal<boolean>(false);
  readonly isMobile = signal<boolean>(false);
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
