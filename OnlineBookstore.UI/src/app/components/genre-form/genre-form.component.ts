import { Component, Input } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { GenresService } from 'src/app/services/genres-service.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'genre-form',
  templateUrl: './genre-form.component.html',
  styleUrls: ['./genre-form.component.css']
})
export class GenreFormComponent {
  @Input()
  genreId? :number;
  genreForm = new FormGroup({
    name: new FormControl('', Validators.required),
    description: new FormControl('')
  });

  constructor(
    private genreService: GenresService
  ) {}

  updateGenre(): void {
    if(this.genreForm.invalid || !this.genreId) {
      this.throwErrorAlert();
      return;
    }
    this.genreService
      .updateGenre(this.genreId!, this.genreForm.get('name')!.value!, this.genreForm.get('description')!.value ?? '')
      .subscribe({
        next: () => {
          Swal.fire({
            position: "bottom-end",
            icon: "success",
            title: "Genre updated",
            showConfirmButton: false,
            timer: 2500
          });
          return;
        },
        error: () => {
          this.throwErrorAlert();
          return;
        }
      });
  }

  createGenre(): void {
    if(this.genreForm.invalid) {
      this.throwErrorAlert();
      return;
    }
    this.genreService
      .createGenre(this.genreForm.get('name')!.value!, this.genreForm.get('description')!.value ?? '')
      .subscribe({
        next: () => {
          Swal.fire({
            position: "bottom-end",
            icon: "success",
            title: "Genre created",
            showConfirmButton: false,
            timer: 2500
          });
          return;
        },
        error: () => {
          this.throwErrorAlert();
          return;
        }
      });
  }
  
  private throwErrorAlert(): void {
    Swal.fire({
      position: "bottom-end",
      icon: "error",
      title: "Error creating. Enter valid data",
      showConfirmButton: false,
      timer: 2500
    });
  }
}
