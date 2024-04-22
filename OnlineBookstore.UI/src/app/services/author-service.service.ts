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
    const apiUrl = environment.apiBaseUrl + environment.endpoints.author.authorBasePath + id;

    return this.http.get<AuthorModel>(apiUrl);
  }

  getAllAuthors(): Observable<AuthorModel[]> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.author.authorBasePath;

    return this.http.get<AuthorModel[]>(apiUrl);
  }

  deleteAuthor(authorId: number) {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.author.authorBasePath;
    let params = new HttpParams().set('authorId', authorId);

    return this.http.delete(apiUrl, { params });
  }
}
