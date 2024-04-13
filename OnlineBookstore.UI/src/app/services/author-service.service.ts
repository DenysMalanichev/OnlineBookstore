import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorModel } from '../models/author-models/authorModel';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthorService {

  constructor(private http: HttpClient) { }

  getAuthorById(id: number): Observable<AuthorModel> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.author.authorBasePath;
    let param = new HttpParams().set('authorId', id);

    return this.http.get<AuthorModel>(apiUrl, { params: param });
  }
}
