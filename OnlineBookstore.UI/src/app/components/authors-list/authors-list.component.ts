import { Component, OnInit } from '@angular/core';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { AuthorService } from 'src/app/services/author-service.service';

@Component({
  selector: 'app-authors-list',
  templateUrl: './authors-list.component.html',
  styleUrls: ['./authors-list.component.css']
})
export class AuthorsListComponent implements OnInit {
  authors!: AuthorModel[];
  
  constructor(private authorsService: AuthorService) {}

  ngOnInit(): void {
    this.getAuthors();
  }

  getAuthors() {
    this.authorsService.getAllAuthors().subscribe(x => this.authors = x);
  }
}
