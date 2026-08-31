import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from './services/auth.service';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet, MatIconModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(
    public authService: AuthService,
    public themeService: ThemeService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  async openProfile(): Promise<void> {
    const { ProfileComponent } = await import('./pages/profile/profile.component');
    this.dialog.open(ProfileComponent, {
      width: '600px',
      maxWidth: '95vw',
      autoFocus: false
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
