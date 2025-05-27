import { Component, ViewChild } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms'

import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { GetFilteredBooksRequest } from 'src/app/models/book-models/getFilteredBooksRequest';
import { BriefGenreModel } from 'src/app/models/genre-models/briefGenreModel';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
import { BooksService } from 'src/app/services/books-service.service';
import { GenresService } from 'src/app/services/genres-service.service';
import { PublishersService } from 'src/app/services/publishers-service.service';

@Component({
  selector: 'books-filter-container',
  templateUrl: './books-filter.component.html',
  styleUrls: ['./books-filter.component.css']
})
export class BooksFilterComponent {
  @ViewChild('carouselRef') carouselComponent: any;

  personalInfoLabel: string = 'Personal Recommendations \u24D8';
  genres: BriefGenreModel[] = [];
  publishers: BriefPublisherModel[] = [];
  books: BriefBookModel[] = [];
  recommendedBooks: BriefBookModel[] = [];

  cols = 3;
  totalPages: number = 0;
  currentRecommendationsPage = 1;
  
  getFilteredBooksRequest = new FormGroup({
    name: new FormControl(null),
    authorName: new FormControl(null),
    publisherId: new FormControl(null),
    isDescending: new FormControl(false),
    minPrice: new FormControl(0),
    maxPrice: new FormControl(9999),
    page: new FormControl(1),
    itemsOnPage: new FormControl(9),
    genreIds: new FormControl(null)
  });

  responsiveOptions: any[] | undefined;

  constructor(
    private booksService: BooksService,
    private genresService: GenresService,
    private publishersService: PublishersService
    ) {}

  ngOnInit(): void {
    this.getFilteredBooks();
    this.getAllGenres();
    this.getAllPublishers();
    this.getRecommendedBooks();

    this.responsiveOptions = [
      {
        breakpoint: '1199px',
        numVisible: 1,
        numScroll: 1
      },
      {
        breakpoint: '991px',
        numVisible: 2,
        numScroll: 1
      },
      {
        breakpoint: '767px',
        numVisible: 1,
        numScroll: 1
      }
    ];
  }

  protected getFilteredBooks() {
    this.booksService.getFilteredBooks(this.prepareFilteredBooksRequest(this.getFilteredBooksRequest.value))
      .subscribe(x => {
        this.books = x.entities;
        this.totalPages = x.totalPages;
      });
  }

  protected getRecommendedBooks(event?: any) {
    if(!event || event.page >= this.currentRecommendationsPage) {
      this.booksService.getRecommendedBooks(this.carouselComponent?.page ?? 1, (this.carouselComponent?.numVisible ?? 4) + 1)
        .subscribe(x => {
            this.recommendedBooks = this.recommendedBooks.concat(x.entities);
        });

      this.currentRecommendationsPage++;  
    }
  }

  private getAllGenres() {
    this.genresService.getAllGenres()
      .subscribe(x => this.genres = x);
  }

  private getAllPublishers() {
    this.publishersService.getAllPublishers()
      .subscribe(x => this.publishers = x);
  }

  private prepareFilteredBooksRequest(formValue: any): GetFilteredBooksRequest {
    return {
      name: formValue.name,
      authorName: formValue.authorName,
      publisherId: formValue.publisherId?.id,
      isDescending: formValue.isDescending || false,
      minPrice: formValue.minPrice,
      maxPrice: formValue.maxPrice,
      page: formValue.page || 1,
      itemsOnPage: formValue.itemsOnPage || 9,
      genres: formValue.genreIds?.map((g: { id: any; }) => g.id)
    };
  }
}