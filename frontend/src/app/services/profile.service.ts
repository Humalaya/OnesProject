import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE } from '../core/api-config';

export interface Profile {
  id: string;
  username: string;
  email: string;
  fullName: string | null;
  profilePictureUrl: string | null;
  emailVerified: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = `${API_BASE}/profile`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<Profile> {
    return this.http.get<Profile>(this.apiUrl);
  }

  updateUsername(username: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/username`, { username });
  }

  updateFullName(fullName: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/fullname`, { fullName });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/password`, { currentPassword, newPassword });
  }

  resendVerification(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/resend-verification`, {});
  }

  uploadPicture(file: File): Observable<{ profilePictureUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ profilePictureUrl: string }>(`${this.apiUrl}/picture`, formData);
  }
}
