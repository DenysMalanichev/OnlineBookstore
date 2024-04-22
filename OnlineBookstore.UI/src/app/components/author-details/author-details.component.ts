import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { AuthService } from 'src/app/services/auth.service';
import { AuthorService } from 'src/app/services/author-service.service';
import { BooksService } from 'src/app/services/books-service.service';
import Swal from 'sweetalert2';

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

  isAdmin = false;

  constructor(
    private authorService: AuthorService,
    private booksService: BooksService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = +params['id'];
      if (id) {
        this.getAuthor(id).subscribe(a => {
          this.author = a;
          this.getBooksByAuthor(a.id, this.page);          
        });
      }
      this.isAdminCheck();
    });    
  }

  getAuthor(id: number): Observable<AuthorModel> {
    return this.authorService.getAuthorById(id);
  }

  deleteAuthor(authorId: number): void {
    this.authorService.deleteAuthor(authorId).subscribe({
      next: () => { 
          Swal.fire({
          position: "bottom-end",
          icon: "success",
          title: "Deleted",
          showConfirmButton: false,
          timer: 2500
        });
        this.router.navigate(['/authors']);
        return;
      },
      error: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "error",
          title: "Error deleting. Make sure you`ve deleted all books of this author before",
          showConfirmButton: false,
          timer: 2500
        });
        return;
      }
    });
  }
  
  getBooksByAuthor(authorId: number, page: number, itemsOnPage = 10): void {
    this.booksService.getBooksByAuthor(authorId, page, itemsOnPage).subscribe(x => {
      this.authoredBooks = x.entities;
      this.totalPages = x.totalPages;
    },
    error => console.error('Error fetching books:', error));
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
