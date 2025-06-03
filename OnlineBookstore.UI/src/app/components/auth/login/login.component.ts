import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = new FormControl('', [
    Validators.required,
    Validators.email
  ]);
  password = new FormControl('', [
    Validators.minLength(8)
  ]);
  constructor(
    private authService: AuthService,
    private router: Router,
    public dialog: MatDialog) { }

  login() {
    if(!this.email.valid || !this.password.valid) {
      this.email.markAsTouched();
      this.password.markAsTouched();
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
