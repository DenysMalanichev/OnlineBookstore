import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { GetFilteredBooksRequest } from '../models/book-models/getFilteredBooksRequest';
import { Observable } from 'rxjs';
import { BriefBookModel } from '../models/book-models/briefBookModel';
import { environment } from '../../environments/environment.development';
import { PageableResponse } from '../models/common/pageableResponse';
import { FullBookModel } from '../models/book-models/fullBookModel';
import { CreateNewBook } from '../models/book-models/createNewBook';
import { UpdateBookModel } from '../models/book-models/updateBookModel';

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

  getRecommendedBooks(page: number, itemsOnPage: number): Observable<PageableResponse<BriefBookModel>> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath + environment.endpoints.books.recommendedBooks;
    let param = new HttpParams()
      .set('page', page.toString())
      .set('itemsOnPage', itemsOnPage.toString());

    return this.http.get<PageableResponse<BriefBookModel>>(apiUrl, { params: param });
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

  getBooksByPublisher(publisherId: number, page: number, itemsOnPage = 10): Observable<PageableResponse<BriefBookModel>> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath + environment.endpoints.books.getBookByPublisher;

    let params = new HttpParams()
      .set('publisherId', publisherId.toString())
      .set('page', page.toString())
      .set('itemsOnPage', itemsOnPage.toString());

    return this.http.get<PageableResponse<BriefBookModel>>(apiUrl, { params: params });
  }

  getBooksAvgRating(bookId: number): Observable<number> {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath + environment.endpoints.books.getBooksAvgRating + bookId;

    return this.http.get<number>(apiUrl);
  }

  addNewBook(book: CreateNewBook) {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath;

    return this.http.post(apiUrl, { ...book });
  }

  updateBook(book: UpdateBookModel) {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath;

    return this.http.put(apiUrl, { ...book });
  }

  deleteBook(bookId: number) {
    const apiUrl = environment.apiBaseUrl + environment.endpoints.books.booksBasePath;
    let params = new HttpParams()
      .set('bookId', bookId.toString())
    return this.http.delete(apiUrl, { params });
  }
}
