import { Component, OnInit } from '@angular/core';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { AuthService } from 'src/app/services/auth.service';
import { AuthorService } from 'src/app/services/author-service.service';

@Component({
  selector: 'app-authors-list',
  templateUrl: './authors-list.component.html',
  styleUrls: ['./authors-list.component.css']
})
export class AuthorsListComponent implements OnInit {
  authors!: AuthorModel[];
  isAdd = false;
  isAdmin = false;
  
  constructor(
    private authorsService: AuthorService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.getAuthors();
    this.isAdminCheck();
  }

  getAuthors() {
    this.authorsService.getAllAuthors().subscribe(x => this.authors = x);
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
