import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  username = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';
  isSubmitting = false;

  constructor(private authService: AuthService, private router: Router) {}

  submit(): void {
    this.errorMessage = '';

    if (!this.username.trim() || !this.email.trim() || !this.password) return;

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters.';
      return;
    }

    this.isSubmitting = true;

    this.authService.register(this.username.trim(), this.email.trim(), this.password).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err.status === 409
          ? (err.error?.message || 'That email or username is already taken.')
          : 'Something went wrong. Please try again.';
      }
    });
  }
}
