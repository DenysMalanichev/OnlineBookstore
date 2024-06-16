import { Component, Input } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { OrdersService } from 'src/app/services/orders.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'make-order',
  templateUrl: './make-order.component.html',
  styleUrls: ['./make-order.component.css']
})
export class MakeOrderComponent {
  @Input()
  bookId!: number;

  bookQuantity = new FormGroup({
    quantity: new FormControl(1, {
      validators: Validators.min(1),
    })
  });
  
  constructor(
    private ordersService: OrdersService,
    private router: Router
    ) {}

  addToOrder(): void {
    const quantityValue = this.bookQuantity.get('quantity')!.value;
    console.log(quantityValue);
   
    if(this.bookQuantity.invalid) {
      Swal.fire({
        position: "bottom-end",
        icon: "error",
        title: "Please, enter valid quantity",
        showConfirmButton: false,
        timer: 2500
      });
      return;
    }

    this.ordersService.addDetailToOrder(this.bookId, quantityValue!)
    .subscribe({
      next: () => {
        this.router.navigate(['/busket']);
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
}
