import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment.development';
import { BriefGenreModel } from '../models/genre-models/briefGenreModel';
import { Observable } from 'rxjs/internal/Observable';
import { FullGenreModel } from '../models/genre-models/fullGenreModel';

@Injectable({
  providedIn: 'root'
})
export class GenresService {

  constructor(private http: HttpClient) { }

  getAllGenres(): Observable<FullGenreModel[]> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.genres.genresBasePath;
    
    return this.http.get<FullGenreModel[]>(apiUrl);
  }

  getGenresByBook(bookId: number): Observable<BriefGenreModel[]> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.genres.genresBasePath + environment.endpoints.genres.genresByBook + bookId;
    
    return this.http.get<BriefGenreModel[]>(apiUrl);
  }

  deleteGenre(genreId: number) {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.genres.genresBasePath;

    let params = new HttpParams().set('genreId', genreId);

    return this.http.delete(apiUrl, { params });
  }
}
