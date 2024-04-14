import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent {

  constructor(
    private authService: AuthService,
    private router: Router
    ) {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
