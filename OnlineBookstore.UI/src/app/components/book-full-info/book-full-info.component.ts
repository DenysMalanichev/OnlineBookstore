import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { FullBookModel } from 'src/app/models/book-models/fullBookModel';
import { BriefGenreModel } from 'src/app/models/genre-models/briefGenreModel';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
import { AuthService } from 'src/app/services/auth.service';
import { AuthorService } from 'src/app/services/author-service.service';
import { BooksService } from 'src/app/services/books-service.service';
import { GenresService } from 'src/app/services/genres-service.service';
import { PublishersService } from 'src/app/services/publishers-service.service';

@Component({
  selector: 'app-book-full-info',
  templateUrl: './book-full-info.component.html',
  styleUrls: ['./book-full-info.component.css']
})
export class BookFullInfoComponent implements OnInit {
  bookId!: number;

  book?: FullBookModel;
  author!: AuthorModel;
  publisher!: BriefPublisherModel;
  genres!: BriefGenreModel[];

  avgRating!: number;

  isToOrder = false;

  isAdmin: boolean = false;
  isUpdateBook: boolean = false;

  constructor(
    private booksService: BooksService,
    private authorService: AuthorService,
    private publisherService: PublishersService,
    private genresService: GenresService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router) {}

  ngOnInit(): void {    
    this.route.params.subscribe((params) => {
      this.bookId = +params['id'];
      if (this.bookId) {
        this.getBook(this.bookId).subscribe(b => {
          this.book = b;
          this.getAuthor(b.authorId);
          this.getPublisher(b.publisherId);
          this.getGenresByBook(this.bookId);
          this.getBooksAvgRating(this.bookId);
        });
      }

      this.isAdminCheck();
    });    
  }

  deleteBook(): void {
    this.booksService.deleteBook(this.bookId).subscribe({
      next: () => {
        this.router.navigate(['/books-filters'])
      }
    });
  }

  getBook(id: number): Observable<FullBookModel> {
    return this.booksService.getBookById(id);
  }

  getAuthor(id: number): void {
    this.authorService.getAuthorById(id).subscribe(x => this.author = x);
  }

  getPublisher(id: number): void {
    this.publisherService.getPublisherById(id).subscribe(x => this.publisher = x);
  }

  getGenresByBook(bookId: number): void {
    this.genresService.getGenresByBook(bookId).subscribe(x => this.genres = x);
  }

  getBooksAvgRating(bookId: number): void {
    this.booksService.getBooksAvgRating(bookId).subscribe(x => this.avgRating = x);    
  } 

  get genreNames(): string {
    return this.genres.map(g => g.name).join(', ');
  }

  isAdminCheck(): void {    
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
