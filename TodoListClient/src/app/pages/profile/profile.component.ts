import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ProfileService, Profile } from '../../services/profile.service';
import { AuthService } from '../../services/auth.service';

const API_ORIGIN = 'http://localhost:5000';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profile: Profile | null = null;

  usernameInput = '';
  usernameMessage = '';
  usernameError = '';
  isSavingUsername = false;

  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  passwordMessage = '';
  passwordError = '';
  isSavingPassword = false;

  selectedFile: File | null = null;
  previewUrl: string | null = null;
  pictureError = '';
  isUploadingPicture = false;

  constructor(
    private profileService: ProfileService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.usernameInput = profile.username;
      },
      error: (err) => console.error('Error loading profile:', err)
    });
  }

  pictureUrl(url: string | null): string | null {
    if (!url) return null;
    return url.startsWith('http') ? url : `${API_ORIGIN}${url}`;
  }

  saveUsername(): void {
    if (!this.usernameInput.trim() || !this.profile) return;

    this.usernameMessage = '';
    this.usernameError = '';
    this.isSavingUsername = true;

    this.profileService.updateUsername(this.usernameInput.trim()).subscribe({
      next: () => {
        this.isSavingUsername = false;
        this.usernameMessage = 'Username updated.';
        this.profile!.username = this.usernameInput.trim();
        this.authService.updateStoredUser({ username: this.profile!.username });
      },
      error: (err) => {
        this.isSavingUsername = false;
        this.usernameError = err.status === 409
          ? 'That username is already taken.'
          : 'Could not update username.';
      }
    });
  }

  changePassword(): void {
    this.passwordMessage = '';
    this.passwordError = '';

    if (!this.currentPassword || !this.newPassword) return;

    if (this.newPassword !== this.confirmPassword) {
      this.passwordError = 'New password and confirmation do not match.';
      return;
    }

    this.isSavingPassword = true;

    this.profileService.changePassword(this.currentPassword, this.newPassword).subscribe({
      next: () => {
        this.isSavingPassword = false;
        this.passwordMessage = 'Password updated.';
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
      },
      error: (err) => {
        this.isSavingPassword = false;
        this.passwordError = err.status === 400
          ? 'Current password is incorrect.'
          : 'Could not update password.';
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.selectedFile = file;
    this.pictureError = '';

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
    this.pictureError = '';

    this.profileService.uploadPicture(this.selectedFile).subscribe({
      next: (result) => {
        this.isUploadingPicture = false;
        this.profile!.profilePictureUrl = result.profilePictureUrl;
        this.authService.updateStoredUser({ profilePictureUrl: result.profilePictureUrl });
        this.selectedFile = null;
        this.previewUrl = null;
      },
      error: () => {
        this.isUploadingPicture = false;
        this.pictureError = 'Could not upload picture.';
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
