import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  confirmPassword: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  token: string;
  refreshToken: string;
}

export interface RegisterResponse {
  message: string;
}

export interface RefreshTokenResponse {
  refreshToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly apiUrl = 'http://localhost:8080/api';
  private readonly tokenKey = 'auth_token';
  private readonly refreshTokenKey = 'refresh_token';

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, credentials).pipe(
      tap(res => {
        this.setToken(res.token);
        this.setRefreshToken(res.refreshToken);
      })
    );
  }

  register(data: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/Auth/register`, data);
  }

  refresh(refreshToken: string): Observable<RefreshTokenResponse> {
    return this.http.post<RefreshTokenResponse>(`${this.apiUrl}/Auth/refresh`, JSON.stringify(refreshToken), {
      headers: { 'Content-Type': 'application/json' }
    }).pipe(
      tap(res => this.setRefreshToken(res.refreshToken))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private setRefreshToken(token: string): void {
    localStorage.setItem(this.refreshTokenKey, token);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token;
  }

  welcome(): Observable<any> {
    const token = this.getToken();
    return this.http.get(`${this.apiUrl}/Auth/welcome`, {
      headers: { Authorization: `Bearer ${token}` },
      responseType: 'json'
    });
  }
}
