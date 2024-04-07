import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment.development';
import { BriefPublisherModel } from '../models/publisher-models/briefPublisherModel';

@Injectable({
  providedIn: 'root'
})
export class PublishersService {

  constructor(private http: HttpClient) { }

  getAllPublishers(): Observable<BriefPublisherModel[]> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.publishers.publishersBasePath;
    
    return this.http.get<BriefPublisherModel[]>(apiUrl);
  }
}
