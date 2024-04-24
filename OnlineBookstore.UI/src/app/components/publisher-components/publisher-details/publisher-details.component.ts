import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { FullPublisherModel } from 'src/app/models/publisher-models/fullPublisherModel';
import { AuthService } from 'src/app/services/auth.service';
import { BooksService } from 'src/app/services/books-service.service';
import { PublishersService } from 'src/app/services/publishers-service.service';
import Swal from 'sweetalert2';

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

  isAdmin = false;
  isUpdate = false;

  constructor(
    private publishersService: PublishersService,
    private booksService: BooksService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
    ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = +params['id'];
      if (id) {
        this.getPublisher(id).subscribe(p => {
          this.publisher = p;
          this.getBooksByPublisher(id, this.page);          
        });
      }

      this.isAdminCheck();
    });   
  }

  getPublisher(id: number): Observable<FullPublisherModel> {
    return this.publishersService.getPublisherById(id);
  }

  deletePublisher(publisherId: number): void {
    this.publishersService.deletePublisher(publisherId).subscribe({
      next: () => { 
          Swal.fire({
          position: "bottom-end",
          icon: "success",
          title: "Deleted",
          showConfirmButton: false,
          timer: 2500
        });
        this.router.navigate(['/publishers']);
        return;
      },
      error: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "error",
          title: "Error deleting. Make sure you`ve deleted all books of this publisher before",
          showConfirmButton: false,
          timer: 2500
        });
        return;
      }
    });
  }

  getBooksByPublisher(publisherId: number, page: number, itemsOnPage = 10): void {
    this.booksService.getBooksByPublisher(publisherId, page, itemsOnPage)
    .subscribe(x => {
      this.publishedBooks = x.entities;
      this.totalPages = x.totalPages;
    });
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
