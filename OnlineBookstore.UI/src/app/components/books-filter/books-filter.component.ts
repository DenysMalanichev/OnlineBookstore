import { Component } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';

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
  genres: BriefGenreModel[] = [];
  publishers: BriefPublisherModel[] = [];
  books: BriefBookModel[] = [];

  cols = 3;
  totalPages: number = 0;
  
  getFilteredBooksRequest = new FormGroup({
    name: new FormControl(null),
    authorName: new FormControl(null),
    publisherId: new FormControl(null),
    isDescending: new FormControl(false),
    minPrice: new FormControl(null),
    maxPrice: new FormControl(null),
    page: new FormControl(1),
    itemsOnPage: new FormControl(10),
    genres: new FormArray([])
  });

  constructor(
    private booksService: BooksService,
    private genresService: GenresService,
    private publishersService: PublishersService
    ) {}

  ngOnInit(): void {
    this.getFilteredBooks();
    this.getAllGenres();
    this.getAllPublishers();
  }

  protected getFilteredBooks() {
    this.booksService.getFilteredBooks(this.prepareFilteredBooksRequest(this.getFilteredBooksRequest.value))
    .subscribe(x => {
      this.books = x.entities;
      this.totalPages = x.totalPages;
    });
  }

  protected onGenresChange(event: any, genreId: number): void {
    const genresArray: FormArray = this.getFilteredBooksRequest.get('genres') as FormArray;
  
    if (event.target.checked) {
      genresArray.push(new FormControl(genreId));
    } else {
      let index = genresArray.controls.findIndex(ctrl => ctrl.value === genreId);
      if (index !== -1) {
        genresArray.removeAt(index);
      }
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
      publisherId: formValue.publisherId,
      isDescending: formValue.isDescending || false,
      minPrice: formValue.minPrice,
      maxPrice: formValue.maxPrice,
      page: formValue.page || 1,
      itemsOnPage: formValue.itemsOnPage || 10,
      genres: formValue.genres
    };
  }
}