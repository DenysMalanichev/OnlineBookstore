import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { BriefGenreModel } from 'src/app/models/genre-models/briefGenreModel';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
import { AuthorService } from 'src/app/services/author-service.service';
import { GenresService } from 'src/app/services/genres-service.service';
import { PublishersService } from 'src/app/services/publishers-service.service';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { BooksService } from 'src/app/services/books-service.service';
import { CreateNewBook } from 'src/app/models/book-models/createNewBook';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';
import { FullBookModel } from 'src/app/models/book-models/fullBookModel';
import { UpdateBookModel } from 'src/app/models/book-models/updateBookModel';
import { forkJoin } from 'rxjs/internal/observable/forkJoin';

@Component({
  selector: 'book-form',
  templateUrl: './book-form.component.html',
  styleUrls: ['./book-form.component.css']
})
export class BookFormComponent implements OnInit {

  @Input()
  book?: FullBookModel;
  @Input()
  bookId?: number;
  bookForm!: FormGroup;

  genres: BriefGenreModel[] = [];
  publishers: BriefPublisherModel[] = [];
  authors: AuthorModel[] = [];

  genresArray: number[] = [];

  constructor(
    private booksService: BooksService,
    private genresService: GenresService,
    private publishersService: PublishersService,
    private authorsService: AuthorService,
    private router: Router
    ) {}

    ngOnInit(): void {
      this.initForm();
      forkJoin({
        genres: this.genresService.getAllGenres(),
        publishers: this.publishersService.getAllPublishers(),
        authors: this.authorsService.getAllAuthors()
      }).subscribe({
        next: ({genres, publishers, authors}) => {
          this.genres = genres;
          this.publishers = publishers;
          this.authors = authors;
          this.initGenres();
        },
        error: (error) => {
          console.error('Error loading data:', error);
        }
      });
    }
    
    private initForm(): void {
      this.bookForm = new FormGroup({
        name: new FormControl(this.book?.name || null, Validators.required),
        description: new FormControl(this.book?.description || ''),
        price: new FormControl(this.book?.price || 0, [Validators.required, Validators.min(0)]),
        authorId: new FormControl(this.book?.authorId || 0, Validators.required),
        publisherId: new FormControl(this.book?.publisherId || 0, Validators.required),
        genreIds: new FormArray([])
      });
    }

    private initGenres(): void {
      let genresFormArray = new FormArray<FormControl<number | null>>([]);
      
      for (let genre of this.genres) {
        genresFormArray.push(new FormControl<number>(genre.id));
      }
      
      this.bookForm.setControl('genreIds', genresFormArray);
    }

  addNewBook(): void {
    if(this.bookForm.invalid) {
      Swal.fire({
        position: "bottom-end",
        icon: "error",
        title: "Enter valid data",
        showConfirmButton: false,
        timer: 2500
      });
      return;
    }
    this.booksService.addNewBook(this.createNewBookObject(this.bookForm.value)).subscribe({
      next: () => {
        this.router.navigate(['/book-details/', this.bookId]);
        return;
      },
      error: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "error",
          title: "An error occured",
          showConfirmButton: false,
          timer: 2500
        });
        return;
      }
    });
  }

  updateBook(): void {
    if(this.bookForm.invalid || !this.book || !this.bookId) {
      Swal.fire({
        position: "bottom-end",
        icon: "error",
        title: "Enter valid data",
        showConfirmButton: false,
        timer: 2500
      });
      return;
    }
    this.booksService.updateBook(this.createUpdateBookObject(this.bookForm.value)).subscribe({
      next: () => {
        this.router.navigate(['/books-filters']);
        return;
      },
      error: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "error",
          title: "An error occured",
          showConfirmButton: false,
          timer: 2500
        });
        return;
      }
    });
  }

  protected onGenresChange(event: any, genreId: number): void {  
    if (event.target.checked) {
      this.genresArray.push(genreId);
    } else {
      let index = this.genresArray.findIndex(ctrl => ctrl == genreId);
      if (index !== -1) {
        this.genresArray.splice(index, 1);
      }
    }
  }

  private createNewBookObject(formValue: any): CreateNewBook {
    return {
      name: formValue.name,
      description: formValue.description || '',
      price: formValue.price,
      authorId: formValue.authorId,
      publisherId: formValue.publisherId,
      genreIds: this.genresArray as []
    };
  }

  private createUpdateBookObject(formValue: any): UpdateBookModel {  
    return {
      id: this.bookId!,
      name: formValue.name,
      description: formValue.description || '',
      price: formValue.price,
      authorId: formValue.authorId,
      publisherId: formValue.publisherId,
      genreIds: this.genresArray as []
    };
  }

  get genreControls(): FormArray {
    return this.bookForm.get('genreIds') as FormArray;
  }

  isInBooksGenres(genreId: number): boolean {
    if(!this.book) {
      return false;
    }

    for(let gId of this.book?.genreIds!) {
      if(gId == genreId){
        return true;
      }
    }
    
    return false;
  }
}