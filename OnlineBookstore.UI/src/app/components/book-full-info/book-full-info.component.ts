import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { FileUploadEvent } from 'primeng/fileupload';
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
import { environment } from '../../../environments/environment.development';
import { OrdersService } from 'src/app/services/orders.service';

@Component({
  selector: 'app-book-full-info',
  templateUrl: './book-full-info.component.html',
  styleUrls: ['./book-full-info.component.css']
})
export class BookFullInfoComponent implements OnInit {
  chartData: any;
  chartOptions: any;
  annualPurchases: number = 0;
  annualIncome: string = '';

  bookId!: number;

  book?: FullBookModel;
  bookImageUrl!: SafeUrl;
  author!: AuthorModel;
  publisher!: BriefPublisherModel;
  genres!: BriefGenreModel[];

  avgRating!: number;

  isToOrder = false;

  isAdmin: boolean = false;
  isUpdateBook: boolean = false;

  constructor(
    private booksService: BooksService,
    private ordersService: OrdersService,
    private authorService: AuthorService,
    private publisherService: PublishersService,
    private genresService: GenresService,
    private route: ActivatedRoute,
    private authService: AuthService,
    protected router: Router,
    private sanitizer: DomSanitizer) {}

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
        this.booksService.getBookImage(this.bookId).subscribe(i => {
          if(i && i.size > 0) {
            const imageObjectUrl = URL.createObjectURL(i);
            this.bookImageUrl = this.sanitizer.bypassSecurityTrustUrl(imageObjectUrl);
          }else {
            this.bookImageUrl = 'https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/660px-No-Image-Placeholder.svg.png?20200912122019';
          }
        });

        this.ordersService.getBooksOrderStatistics(this.bookId).subscribe(stats => {
          let data = [];
          for (let month = 1; month <= 12; month++) {
            if(stats.find(s => s.month == month)) {
              data.push(stats.find(s => s.month == month)!.quantity);
              this.annualPurchases += data[month-1];
            }
            else {
              data.push(0);
            }
          }

          this.annualIncome = (this.annualPurchases * this.book!.price).toFixed(2);

          this.chartData = {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            datasets: [
                {
                    label: 'Monthly purchases',
                    data: data,
                    fill: false,
                    tension: 0.5
                }
            ]
          };
          this.chartOptions = {
              maintainAspectRatio: false,
              aspectRatio: 1
          };
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

  getImageUploadUrl(): string {
    const defaultEndpointUri = environment.apiBaseUrl + environment.endpoints.books.booksBasePath + environment.endpoints.books.bookImage;
    let url = defaultEndpointUri + '?bookId=' + this.bookId.toString();
    return url;
  }
}
