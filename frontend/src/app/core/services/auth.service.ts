import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthenticatedUser, LoginRequest, LoginResponse } from '../models/auth.model';
import { ApiService } from './api.service';

const TOKEN_KEY = 'icoms_access_token';
const USER_KEY = 'icoms_current_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = inject(ApiService);

  currentUser = signal<AuthenticatedUser | null>(this.readStoredUser());
  isAuthenticated = computed(() => this.currentUser() !== null);

  login(request: LoginRequest): Observable<LoginResponse> {
    const payload = {
      staffIdOrEmail: request.identifier,
      password: request.password,
      rememberDevice: request.rememberMe
    };
    return this.api.post<LoginResponse>('/auth/login', payload).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_KEY, response.accessToken);
        localStorage.setItem(USER_KEY, JSON.stringify(response.user));
        this.currentUser.set(response.user);
      })
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

  private readStoredUser(): AuthenticatedUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) {
      return null;
    }
    try {
      return JSON.parse(raw) as AuthenticatedUser;
    } catch {
      return null;
    }
  }
}
