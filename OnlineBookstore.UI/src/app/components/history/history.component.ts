import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { FullBookModel } from 'src/app/models/book-models/fullBookModel';
import { GetOrderModel } from 'src/app/models/order-models/GetOrderModel';
import { BooksService } from 'src/app/services/books-service.service';
import { OrdersService } from 'src/app/services/orders.service';

@Component({
  selector: 'orders-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit {
  orders!: GetOrderModel[];
  books: {[id: number] : FullBookModel} = {};

  constructor(
    private ordersService: OrdersService,
    private booksService: BooksService
    ) {}

  ngOnInit(): void {
    this.getHistory().subscribe(x => {
      this.orders = x;
      for(let order of x) {
        this.getBriefBookFromOrder(order);
      }      
    });
  }

  getHistory(): Observable<GetOrderModel[]> {
    return this.ordersService.getHistory();
  }

  getBriefBookFromOrder(order: GetOrderModel): void {
    for(let orderDetail of order.orderDetails) {
      this.booksService.getBookById(orderDetail.bookId).subscribe(x => this.books[orderDetail.bookId] = x);
    }
  }
  countOrderTotalPrice(order: GetOrderModel): number {
    let price = 0;
    for(let orderDetail of order.orderDetails) {
      price += this.books[orderDetail.bookId].price * orderDetail.quantity;
    }

    return price;
  }
}
