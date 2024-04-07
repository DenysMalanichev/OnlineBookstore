import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { GetFilteredBooksRequest } from 'src/app/models/book-models/getFilteredBooksRequest';
import { BooksService } from 'src/app/services/books-service.service';

@Component({
  selector: 'books-list',
  templateUrl: './books-list.component.html',
  styleUrls: ['./books-list.component.css']
})
export class BooksListComponent implements OnInit{
  @Input()
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
    genres: new FormControl(null)
  });

  constructor(
    private booksService: BooksService,
    private breakpointObserver: BreakpointObserver
    ) {}

  ngOnInit(): void {
    this.getFilteredBooks();

    const gridMap = new Map([
      [Breakpoints.XSmall, 1],
      [Breakpoints.Small, 1],
      [Breakpoints.Medium, 2],
      [Breakpoints.Large, 3],
      [Breakpoints.XLarge, 4],
    ]);

    this.breakpointObserver.observe([
      Breakpoints.XSmall,
      Breakpoints.Small,
      Breakpoints.Medium,
      Breakpoints.Large,
      Breakpoints.XLarge,
    ]).subscribe(result => {
      for (const [breakpoint, cols] of gridMap.entries()) {
        if (result.breakpoints[breakpoint]) {
          this.cols = cols;
          break;
        }
      }
    });
  }

  private getFilteredBooks() {
    this.booksService.getFilteredBooks(this.prepareFilteredBooksRequest(this.getFilteredBooksRequest.value))
    .subscribe(x => {
      this.books = x.entities;
      this.totalPages = x.totalPages;
    });
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
