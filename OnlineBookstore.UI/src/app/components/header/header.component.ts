import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit  {

  items: MenuItem[] | undefined;

  constructor(private authService: AuthService) {}
  ngOnInit(): void {
    this.items = [
      {
        label: 'Books',
        icon: 'pi pi-book',
        routerLink: '/books-filters'
      },
      {
        label: 'Genres',
        icon: 'pi pi-star',
        routerLink: '/genres'
      },
      {
        label: 'Authors',
        icon: 'pi pi-user-edit',
        routerLink: '/authors'
      },
      {
        label: 'Publishers',
        icon: 'pi pi-users',
        routerLink: '/publishers'
      }
    ];
  }

  isLoggedin(): boolean {
    return this.authService.loggedIn;
  }
}
