import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';
import { PasswordStrengthMeterComponent } from '../../shared/password-strength-meter/password-strength-meter.component';
import { generateRandomUsername } from '../../shared/random-username';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, PasswordStrengthMeterComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  username = generateRandomUsername();
  email = '';
  password = '';
  confirmPassword = '';
  isSubmitting = false;

  readonly emailPattern = '^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$';

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {}

  suggestUsername(): void {
    this.username = generateRandomUsername();
  }

  submit(): void {
    if (!this.username.trim() || !this.email.trim() || !this.password) return;

    if (this.password !== this.confirmPassword) {
      this.toastr.error('Passwords do not match.');
      return;
    }

    if (this.password.length < 6) {
      this.toastr.error('Password must be at least 6 characters.');
      return;
    }

    this.isSubmitting = true;

    this.authService.register(this.username.trim(), this.email.trim(), this.password).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.toastr.success('Account created! Check your email to verify your address.');
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.toastr.error(err.status === 409
          ? (err.error?.message || 'That email or username is already taken.')
          : (err.error?.message || 'Something went wrong. Please try again.'));
      }
    });
  }
}
