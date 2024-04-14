import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = new FormControl('');
  password = new FormControl('');

  constructor(
    private authService: AuthService,
    private router: Router) { }

  login() {
    if(this.email.value == null || this.password.value == null) {
      alert('enter data!');
      return;
    }
    this.authService.login(this.email.value!, this.password.value!).subscribe({
      next: () => {
        this.router.navigate(['/books-filters']);
      },
      error: (error) => {
        alert('failed!' + error);
      }
    });
  }
}
