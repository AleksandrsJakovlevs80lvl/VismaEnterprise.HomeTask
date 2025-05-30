import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'authToken';

  constructor(private http: HttpClient) { }

  login(credentials: any): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.apiUrl}/Auth/login`, credentials, { observe: 'response' }).pipe(
      tap(response => {
        if (response && response.body && response.body.token) {
          this.setToken(response.body.token);
        }
      })
    );
  }

  register(credentials: any): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.apiUrl}/Auth/register`, credentials, { observe: 'response' });
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }
}
