import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { FullBookModel } from 'src/app/models/book-models/fullBookModel';
import { GetOrderModel } from 'src/app/models/order-models/GetOrderModel';
import { BooksService } from 'src/app/services/books-service.service';
import { OrdersService } from 'src/app/services/orders.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})
export class BusketComponent implements OnInit{

  order!: GetOrderModel;
  books: {[id: number] : FullBookModel} = {};

  shipData = new FormGroup({
    shipCity: new FormControl('', [
        Validators.required
    ]),
    shipAddress: new FormControl('', [
      Validators.required
    ])
  });

  constructor(
    private ordersService: OrdersService,
    private booksService: BooksService,
    private router: Router
    ) {}

  ngOnInit(): void {
    this.getUserOpenOrder()
    .subscribe(x => {
      this.order = x;
      this.getBriefBookFromOrder(x);
    });
  }

  getUserOpenOrder(): Observable<GetOrderModel> {
    return this.ordersService.getUserOpenOrder()
  }

  getBriefBookFromOrder(order: GetOrderModel): void {
    for(let orderDetail of order.orderDetails) {
      this.booksService.getBookById(orderDetail.bookId).subscribe(x => this.books[orderDetail.bookId] = x);
    }
  }

  shipOrder(): void {
    if(this.shipData.invalid) {
      Swal.fire({
        position: "bottom-end",
        icon: "error",
        title: "Enter shipping data",
        showConfirmButton: false,
        timer: 2500
      });
      return;
    }

    this.ordersService.shipUserOrder(this.shipData.get('shipCity')!.value!, this.shipData.get('shipAddress')!.value!)
      .subscribe( {
        next: () => { 
          this.router.navigate(['/account']); 
        },
        error: () => {
          Swal.fire({
            position: "bottom-end",
            icon: "error",
            title: "Something goes wrong. Try again",
            showConfirmButton: false,
            timer: 2500
          });
          return;
        }
      });
  }

  countOrderTotalPrice(): number {
    let price = 0;
    for(let orderDetail of this.order.orderDetails) {
      price += this.books[orderDetail.bookId].price * orderDetail.quantity;
    }

    return price;
  }
}
