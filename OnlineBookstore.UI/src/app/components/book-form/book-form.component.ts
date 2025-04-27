import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { BriefGenreModel } from 'src/app/models/genre-models/briefGenreModel';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
import { AuthorService } from 'src/app/services/author-service.service';
import { GenresService } from 'src/app/services/genres-service.service';
import { PublishersService } from 'src/app/services/publishers-service.service';
import { BooksService } from 'src/app/services/books-service.service';
import { CreateNewBook } from 'src/app/models/book-models/createNewBook';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';
import { FullBookModel } from 'src/app/models/book-models/fullBookModel';
import { UpdateBookModel } from 'src/app/models/book-models/updateBookModel';
import { forkJoin } from 'rxjs/internal/observable/forkJoin';

interface AuthorDisplayModel {
  name: string,
  id: number
}
interface Language {
  name: string,
  code: string
}

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
  authors: AuthorDisplayModel[] = [];

  genresArray: number[] = [];
  languages: Language[] =  [];

  constructor(
    private booksService: BooksService,
    private genresService: GenresService,
    private publishersService: PublishersService,
    private authorsService: AuthorService,
    private router: Router
    ) {
      this.languages = [
        { name: "English", code: "EN"},
        { name: "Українська", code: "UA"},
        { name: "Español", code: "ES" },
        { name: "Français", code: "FR" },
        { name: "Deutsch", code: "DE" },
        { name: "Italiano", code: "IT" },
      ]
    }

    ngOnInit(): void {
      this.initEmptyForm();
      forkJoin({
        genres: this.genresService.getAllGenres(),
        publishers: this.publishersService.getAllPublishers(),
        authors: this.authorsService.getAllAuthors()
      }).subscribe({
        next: ({genres, publishers, authors}) => {
          this.genres = genres;
          this.publishers = publishers;
          this.authors = authors.map(a => ({ 
            name: a.firstName + ' ' + a.lastName, 
            id: a.id 
          }));

          this.updateFormWithData();
        },
        error: (error) => {
          console.error('Error loading data:', error);
        }
      });
      this.updateFormWithData();
    }

    private initEmptyForm(): void {
      this.bookForm = new FormGroup({
        name: new FormControl(this.book?.name || null, Validators.required),
        description: new FormControl(this.book?.description || ''),
        price: new FormControl(this.book?.price || null, [Validators.required, Validators.min(0)]),
        authorId: new FormControl(this.book?.authorId || null, Validators.required),
        publisherId: new FormControl(null, Validators.required),
        genreIds: new FormControl(null, Validators.required),
        isPaperback: new FormControl(this.book?.isPaperback || ''),
        language: new FormControl(this.languages.find(l => l.code === this.book?.language) || null, [Validators.required]),
      });
    }
    
    private updateFormWithData(): void {
      // Find the publisher if we're editing a book
      const selectedPublisher = this.book?.publisherId ? 
        this.publishers.find(p => p.id === this.book?.publisherId) : 
        null;

      // Find the author if we're editing a book
      const selectedAuthor = this.book?.authorId ? 
        this.authors.find(p => p.id === this.book?.authorId) : 
        null;

      // Find genres if we're editing a book
      const selectedGenres = this.book?.genreIds?.length! > 0 ? 
        this.genres.filter(p => this.book?.genreIds?.includes(p.id)) : 
        null;
      
      // Update the form with the loaded data
      this.bookForm.patchValue({
        publisherId: selectedPublisher,
        authorId: selectedAuthor,
        genreIds: selectedGenres
      });
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
      authorId: formValue.authorId.id,
      publisherId: formValue.publisherId.id,
      genreIds: formValue.genreIds.map((g: BriefGenreModel) => g.id) as [],
      isPaperback: formValue.isPaperback,
      language: formValue.language.code
    };
  }

  private createUpdateBookObject(formValue: any): UpdateBookModel {  
    return {
      id: this.bookId!,
      name: formValue.name,
      description: formValue.description || '',
      price: formValue.price,
      authorId: formValue.authorId.id,
      publisherId: formValue.publisherId.id,
      genreIds: formValue.genreIds.map((g: BriefGenreModel) => g.id) as [],
      isPaperback: formValue.isPaperback,
      language: formValue.language.code
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