import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterModel } from 'src/app/models/auth-models/registerModel';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerFormControl = new FormGroup({
    firstName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8)
    ]),
    confirmPassword: new FormControl('', [
      Validators.required
    ]),
  });

  constructor(private authService: AuthService, private router: Router) {}

  register(): void {
    if(this.registerFormControl.invalid || this.passwordMatchValidator()) {
      return;
    }

    const registerModel = this.generateRegisterModel(this.registerFormControl.value);
    this.authService.register(registerModel).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      }
    });
  }

  generateRegisterModel(formValue: any): RegisterModel {
    return {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      password: formValue.password,
      confirmPassword: formValue.confirmPassword
    }
  }

  passwordMatchValidator(): boolean {
    const password = this.registerFormControl?.get('password');
    const confirmPassword = this.registerFormControl?.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return true;
    }
    return false;
  }
}
