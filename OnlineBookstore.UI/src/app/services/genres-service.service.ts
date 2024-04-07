import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment.development';
import { BriefGenreModel } from '../models/genre-models/briefGenreModel';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export class GenresService {

  constructor(private http: HttpClient) { }

  getAllGenres(): Observable<BriefGenreModel[]> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.genres.genresBasePath;
    
    return this.http.get<BriefGenreModel[]>(apiUrl);
  }
}
