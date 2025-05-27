import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, interval, fromEvent, merge, timer } from 'rxjs';
import { map, switchMap, tap, filter, throttleTime, take } from 'rxjs/operators';
import { parseJwt } from './claims';

interface LoginResponse {
  token: string;
  expiresIn: number; // segundos
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly API_URL = 'http://localhost:5000/security/api/Auth';
  private readonly TOKEN_KEY = 'access_token';

  private inactivityTime = 3 * 60 * 1000; // 3 minutos
  private refreshInterval: any;

  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor() {
    this.monitorUserActivity();
    this.autoRefreshToken();
  }

login(credentials: { username: string; password: string }): Observable<void> {
  return this.http.post<LoginResponse>(`${this.API_URL}/login`, credentials).pipe(
    tap(response => {
      const expiresAt = Date.now() + (response.expiresIn * 1000);
      const data = {
        token: response.token,
        expiresAt
      };
      localStorage.setItem(this.TOKEN_KEY, JSON.stringify(data));
      this.isLoggedInSubject.next(true);
    }),
    map(() => {})
  );
}

  getCurrentUser(): { userId: string, username: string, role: string } | null {
  const token = this.getAccessToken();
  if (!token) return null;
  const decoded = parseJwt(token);
  return {
    userId: decoded?.nameid,
    username: decoded?.unique_name,
    role: decoded?.role
  };
}

  logout(): void {
    this.clearToken();
    this.isLoggedInSubject.next(false);
    this.router.navigate(['/login']);
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

getAccessToken(): string | null {
  const stored = localStorage.getItem(this.TOKEN_KEY);
  if (!stored) return null;
  try {
    return JSON.parse(stored)?.token ?? null;
  } catch {
    return null;
  }
}private setToken(token: string, expiresIn: number): void {
  const expiresAt = Date.now() + (expiresIn * 1000);
  const data = { token, expiresAt };
  localStorage.setItem(this.TOKEN_KEY, JSON.stringify(data));
}

  private clearToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  private hasToken(): boolean {
    return !!this.getAccessToken();
  }

 refreshToken(): Observable<void> {
  return this.http.post<LoginResponse>(`${this.API_URL}/refreshToken`, {}).pipe(
    tap(res => {
      this.setToken(res.token, res.expiresIn);
      this.isLoggedInSubject.next(true);
    }),
    map(() => {})
  );
}
  private autoRefreshToken(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }

    this.refreshInterval = setInterval(() => {
      this.refreshToken().subscribe({
        next: () => console.log('Token refrescado'),
        error: () => {
          console.warn('Error al refrescar el token, cerrando sesión');
          this.logout();
        }
      });
    }, 2.5 * 60 * 1000); // 2.5 minutos
  }

  getTokenExpiration(): Date | null {
  const token = this.getAccessToken();
  if (!token) return null;
  const decoded = parseJwt(token);
  if (!decoded?.exp) return null;
  return new Date(decoded.exp * 1000); // viene en segundos
  }

  getSecondsUntilExpiration(): number {
    const exp = this.getTokenExpiration();
    if (!exp) return 0;
    return Math.floor((exp.getTime() - Date.now()) / 1000);
  }

  private monitorUserActivity(): void {
    const activityEvents = merge(
      fromEvent(document, 'mousemove'),
      fromEvent(document, 'keydown'),
      fromEvent(document, 'click'),
      fromEvent(document, 'touchstart')
    );

    activityEvents.pipe(
      throttleTime(1000),
      tap(() => {
        // cada vez que hay actividad, reiniciamos el autoRefresh
        this.autoRefreshToken();
      })
    ).subscribe();

    // Inactividad total por más de 3 minutos = logout
    timer(this.inactivityTime).pipe(
      switchMap(() => activityEvents.pipe(take(1))),
      tap(() => this.logout())
    ).subscribe();
  }
}
