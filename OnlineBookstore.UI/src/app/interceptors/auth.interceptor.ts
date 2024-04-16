import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    const token = localStorage.getItem('access_token');

    if (token) {
      const clonedReq = request.clone({ headers: request.headers.set('Authorization', `${token}`) });

      return next.handle(clonedReq);
    }

    return next.handle(request);
  }
}
