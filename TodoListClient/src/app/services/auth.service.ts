import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface AuthUser {
  userId: string;
  username: string;
  email: string;
  profilePictureUrl: string | null;
}

interface AuthResponse extends AuthUser {
  token: string;
}

const TOKEN_KEY = 'todo_auth_token';
const USER_KEY = 'todo_auth_user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/auth';

  currentUser = signal<AuthUser | null>(this.readStoredUser());

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap((response) => this.storeSession(response))
    );
  }

  register(username: string, email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, { username, email, password }).pipe(
      tap((response) => this.storeSession(response))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUser.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  updateStoredUser(patch: Partial<AuthUser>): void {
    const current = this.currentUser();
    if (!current) return;
    const updated = { ...current, ...patch };
    localStorage.setItem(USER_KEY, JSON.stringify(updated));
    this.currentUser.set(updated);
  }

  private storeSession(response: AuthResponse): void {
    const { token, ...user } = response;
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
  }

  private readStoredUser(): AuthUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      return null;
    }
  }
}
