import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  isSubmitting = false;

  readonly emailPattern = '^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$';

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {}

  submit(): void {
    if (!this.email.trim() || !this.password.trim()) return;

    this.isSubmitting = true;

    this.authService.login(this.email, this.password).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.toastr.error(err.status === 401
          ? 'Invalid email or password.'
          : 'Something went wrong. Please try again.');
      }
    });
  }
}
