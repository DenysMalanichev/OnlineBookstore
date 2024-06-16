import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GetUserModel } from 'src/app/models/auth-models/getUserModel';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit{

  user!: GetUserModel;

  constructor(
    private authService: AuthService,
    private router: Router
    ) {}

  ngOnInit(): void {
    this.getUserData();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getUserData() : void {
    this.authService.getUserData().subscribe(x => this.user = x);
  }
}
