import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { FullPublisherModel } from 'src/app/models/publisher-models/fullPublisherModel';
import { BooksService } from 'src/app/services/books-service.service';
import { PublishersService } from 'src/app/services/publishers-service.service';

@Component({
  selector: 'app-publisher-details',
  templateUrl: './publisher-details.component.html',
  styleUrls: ['./publisher-details.component.css']
})
export class PublisherDetailsComponent implements OnInit {  
  publisher!: FullPublisherModel;
  publishedBooks!: BriefBookModel[];

  page = 1;
  totalPages?: number;

  constructor(
    private publishersService: PublishersService,
    private booksService: BooksService,
    private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = +params['id'];
      if (id) {
        this.getPublisher(id).subscribe(p => {
          this.publisher = p;
          this.getBooksByPublisher(id, this.page);          
        });
      }
    });   
  }

  getPublisher(id: number): Observable<FullPublisherModel> {
    return this.publishersService.getPublisherById(id);
  }

  getBooksByPublisher(publisherId: number, page: number, itemsOnPage = 10): void {
    this.booksService.getBooksByPublisher(publisherId, page, itemsOnPage)
    .subscribe(x => {
      console.log(x);
      this.publishedBooks = x.entities;
      this.totalPages = x.totalPages;
    });
  }
}
