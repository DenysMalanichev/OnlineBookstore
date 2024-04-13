import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { GetFilteredBooksRequest } from '../models/book-models/getFilteredBooksRequest';
import { Observable } from 'rxjs';
import { BriefBookModel } from '../models/book-models/briefBookModel';
import { environment } from '../../environments/environment.development';
import { PageableResponse } from '../models/common/pageableResponse';
import { FullBookModel } from '../models/book-models/fullBookModel';

@Injectable({
  providedIn: 'root'
})
export class BooksService {

  constructor(private http: HttpClient) { }

  getFilteredBooks(filters: GetFilteredBooksRequest): Observable<PageableResponse<BriefBookModel>> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath + environment.endpoints.books.getFilteredBooks;
    let params = new HttpParams();
  
    Object.keys(filters).forEach((key) => {
      const filterKey = key as keyof GetFilteredBooksRequest;
      let value = filters[filterKey];
  
    if (value !== null && value !== undefined && Array.isArray(value)) {
          value.forEach(item => {
            params = params.append(filterKey, item.toString());
          });        
    }
    else if(value !== null && value !== undefined){
        params = params.set(filterKey, value.toString());
    }
    });
  
    return this.http.get<PageableResponse<BriefBookModel>>(apiUrl, { params });
  }

  getBookById(id: number): Observable<FullBookModel> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath;
    let param = new HttpParams().set('bookId', id.toString());

    return this.http.get<FullBookModel>(apiUrl, { params: param });
  }

  getBooksByAuthor(authorId: number, page: number, itemsOnPage = 10): Observable<PageableResponse<BriefBookModel>> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath + environment.endpoints.books.getBookByAuthor;

    let params = new HttpParams()
      .set('authorId', authorId.toString())
      .set('page', page.toString())
      .set('itemsOnPage', itemsOnPage.toString());

    return this.http.get<PageableResponse<BriefBookModel>>(apiUrl, { params: params });
  }
}
