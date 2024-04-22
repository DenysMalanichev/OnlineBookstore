import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment.development';
import { BriefPublisherModel } from '../models/publisher-models/briefPublisherModel';
import { FullPublisherModel } from '../models/publisher-models/fullPublisherModel';

@Injectable({
  providedIn: 'root'
})
export class PublishersService {

  constructor(private http: HttpClient) { }

  getAllPublishers(): Observable<BriefPublisherModel[]> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.publishers.publishersBasePath;
    
    return this.http.get<BriefPublisherModel[]>(apiUrl);
  }

  getPublisherById(id: number): Observable<FullPublisherModel> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.publishers.publishersBasePath + id;
    
    return this.http.get<FullPublisherModel>(apiUrl);
  }

  deletePublisher(publisherId: number) {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.publishers.publishersBasePath;

    let params = new HttpParams().set('publisherId', publisherId);

    return this.http.delete(apiUrl, { params });
  }
}
