import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { tap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { RegisterModel } from '../models/auth-models/registerModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseAuthUrl = environment.apiBaseUrl + environment.endpoints.users.usersBasePath;
  jwtHelper = new JwtHelperService();

  constructor(private http: HttpClient) {}

  login(email: string, password: string) {
    const loginPathUrl = this.baseAuthUrl + environment.endpoints.users.loginUserPath;

    return this.http.post<{token: string}>(loginPathUrl, { email, password }).pipe(
      tap(res => {
        localStorage.setItem('access_token', res.token);
      })
    );
  }

  register(registerModel: RegisterModel) {
    const registerPathUrl = this.baseAuthUrl + environment.endpoints.users.registerUserPath;

    return this.http.post(registerPathUrl, { ...registerModel });
  }

  logout() {
    localStorage.removeItem('access_token');
  }

  public get loggedIn(): boolean {
    let token = localStorage.getItem('access_token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
