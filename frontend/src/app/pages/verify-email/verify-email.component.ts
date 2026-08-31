import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

type VerifyState = 'verifying' | 'success' | 'error';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss']
})
export class VerifyEmailComponent implements OnInit {
  state: VerifyState = 'verifying';
  message = '';

  constructor(private route: ActivatedRoute, private authService: AuthService) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.state = 'error';
      this.message = 'Missing verification token.';
      return;
    }

    this.authService.verifyEmail(token).subscribe({
      next: () => {
        this.state = 'success';
        this.message = 'Your email has been verified.';
        this.authService.updateStoredUser({ emailVerified: true });
      },
      error: (err) => {
        this.state = 'error';
        this.message = err.error?.message || 'This verification link is invalid or has expired.';
      }
    });
  }
}
