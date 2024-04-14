import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { AuthorService } from 'src/app/services/author-service.service';
import { BooksService } from 'src/app/services/books-service.service';

@Component({
  selector: 'app-author-details',
  templateUrl: './author-details.component.html',
  styleUrls: ['./author-details.component.css']
})
export class AuthorDetailsComponent implements OnInit {
  authoredBooks!: BriefBookModel[];
  author!: AuthorModel;

  page = 1;
  totalPages?: number;

  constructor(
    private authorService: AuthorService,
    private booksService: BooksService,
    private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = +params['id'];
      if (id) {
        this.getAuthor(id).subscribe(a => {
          this.author = a;
          this.getBooksByAuthor(a.id, this.page);          
        });
      }
    });    
  }

  getAuthor(id: number): Observable<AuthorModel> {
    return this.authorService.getAuthorById(id);
  }
  
  getBooksByAuthor(authorId: number, page: number, itemsOnPage = 10): void {
    this.booksService.getBooksByAuthor(authorId, page, itemsOnPage).subscribe(x => {
      this.authoredBooks = x.entities;
      this.totalPages = x.totalPages;
    },
    error => console.error('Error fetching books:', error));
  }
}
