import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { FullBookModel } from 'src/app/models/book-models/fullBookModel';
import { BriefGenreModel } from 'src/app/models/genre-models/briefGenreModel';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
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
  book!: FullBookModel;
  author!: AuthorModel;
  publisher!: BriefPublisherModel;
  genres!: BriefGenreModel[];

  constructor(
    private booksService: BooksService,
    private authorService: AuthorService,
    private publisherService: PublishersService,
    private genresService: GenresService,
    private route: ActivatedRoute) {}

  ngOnInit(): void {    
    this.route.params.subscribe((params) => {
      const id = +params['id'];
      if (id) {
        this.getBook(id).subscribe(b => {
          this.book = b;
          this.getAuthor(b.authorId);
          this.getPublisher(b.publisherId);
          this.getGenresByBook(id);
        });
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
    this.publisherService.getBriefPublisherById(id).subscribe(x => this.publisher = x);
  }

  getGenresByBook(bookId: number): void {
    this.genresService.getGenresByBook(bookId).subscribe(x => this.genres = x);
  }

  get genreNames(): string {
    return this.genres.map(g => g.name).join(', ');
  }
}
