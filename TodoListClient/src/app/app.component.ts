import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';

const API_ORIGIN = 'http://localhost:5000';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(public authService: AuthService, private router: Router) {}

  pictureUrl(url: string | null | undefined): string | null {
    if (!url) return null;
    return url.startsWith('http') ? url : `${API_ORIGIN}${url}`;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
