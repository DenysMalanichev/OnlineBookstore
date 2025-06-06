import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { GetOrderModel } from '../models/order-models/GetOrderModel';
import { BookStatisticsModel } from '../models/book-models/BookStatisticsModel';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private baseOrdersPath = environment.apiBaseUrl + environment.endpoints.orders.ordersBasePath;

  constructor(private http: HttpClient) { }

  addDetailToOrder(bookId: number, quantity: number): Observable<any> {
    const addDetailPath = this.baseOrdersPath + environment.endpoints.orders.addDetailToOrderPath;

    return this.http.post(addDetailPath, { bookId, quantity });
  }

  getUserOpenOrder(): Observable<GetOrderModel> {
    const getUserOpenOrderPath = this.baseOrdersPath + environment.endpoints.orders.getUserOpenOrderPath;

    return this.http.get<GetOrderModel>(getUserOpenOrderPath);
  }

  shipUserOrder(shipCity: string, shipAddress: string): Observable<any> {
    const shipOrderPath = this.baseOrdersPath + environment.endpoints.orders.shipUserOrderPath;

    return this.http.post(shipOrderPath, { shipCity, shipAddress });
  }

  getHistory(): Observable<GetOrderModel[]> {
    const shipOrderPath = this.baseOrdersPath + environment.endpoints.orders.getUserHistoryPath;

    return this.http.get<GetOrderModel[]>(shipOrderPath);
  }

  getBooksOrderStatistics(bookId: number): Observable<BookStatisticsModel[]> {
    const getStatisticsPath = this.baseOrdersPath + environment.endpoints.orders.getBooksStatisticsPath + bookId;

    return this.http.get<BookStatisticsModel[]>(getStatisticsPath);
  }
}
