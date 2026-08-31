import { Component, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ProfileService, Profile } from '../../services/profile.service';
import { AuthService } from '../../services/auth.service';
import { PasswordStrengthMeterComponent } from '../../shared/password-strength-meter/password-strength-meter.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, PasswordStrengthMeterComponent],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profile: Profile | null = null;

  fullNameInput = '';
  isSavingFullName = false;

  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  isSavingPassword = false;

  selectedFile: File | null = null;
  previewUrl: string | null = null;
  isUploadingPicture = false;

  isResendingVerification = false;

  constructor(
    private profileService: ProfileService,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService,
    @Optional() public dialogRef: MatDialogRef<ProfileComponent> | null
  ) {}

  ngOnInit(): void {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.fullNameInput = profile.fullName || '';
      },
      error: () => this.toastr.error('Could not load your profile.')
    });
  }

  saveFullName(): void {
    if (!this.profile) return;

    this.isSavingFullName = true;

    this.profileService.updateFullName(this.fullNameInput.trim()).subscribe({
      next: () => {
        this.isSavingFullName = false;
        this.toastr.success('Full name updated.');
        this.profile!.fullName = this.fullNameInput.trim();
      },
      error: () => {
        this.isSavingFullName = false;
        this.toastr.error('Could not update full name.');
      }
    });
  }

  changePassword(): void {
    if (!this.currentPassword || !this.newPassword) return;

    if (this.newPassword !== this.confirmPassword) {
      this.toastr.error('New password and confirmation do not match.');
      return;
    }

    this.isSavingPassword = true;

    this.profileService.changePassword(this.currentPassword, this.newPassword).subscribe({
      next: () => {
        this.isSavingPassword = false;
        this.toastr.success('Password updated.');
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
      },
      error: (err) => {
        this.isSavingPassword = false;
        this.toastr.error(err.status === 400 ? 'Current password is incorrect.' : 'Could not update password.');
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.selectedFile = file;

    if (file) {
      const reader = new FileReader();
      reader.onload = () => (this.previewUrl = reader.result as string);
      reader.readAsDataURL(file);
    } else {
      this.previewUrl = null;
    }
  }

  uploadPicture(): void {
    if (!this.selectedFile || !this.profile) return;

    this.isUploadingPicture = true;

    this.profileService.uploadPicture(this.selectedFile).subscribe({
      next: (result) => {
        this.isUploadingPicture = false;
        this.toastr.success('Profile picture updated.');
        this.profile!.profilePictureUrl = result.profilePictureUrl;
        this.authService.updateStoredUser({ profilePictureUrl: result.profilePictureUrl });
        this.selectedFile = null;
        this.previewUrl = null;
      },
      error: () => {
        this.isUploadingPicture = false;
        this.toastr.error('Could not upload picture.');
      }
    });
  }

  resendVerification(): void {
    this.isResendingVerification = true;
    this.profileService.resendVerification().subscribe({
      next: () => {
        this.isResendingVerification = false;
        this.toastr.success('Verification email sent. Check your inbox.');
      },
      error: () => {
        this.isResendingVerification = false;
        this.toastr.error('Could not resend verification email.');
      }
    });
  }

  close(): void {
    this.dialogRef?.close();
  }

  logout(): void {
    this.dialogRef?.close();
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
