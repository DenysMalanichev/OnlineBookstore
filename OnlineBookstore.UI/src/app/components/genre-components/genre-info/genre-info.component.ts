import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { FullGenreModel } from 'src/app/models/genre-models/fullGenreModel';
import { AuthService } from 'src/app/services/auth.service';
import { GenresService } from 'src/app/services/genres-service.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'genre-info',
  templateUrl: './genre-info.component.html',
  styleUrls: ['./genre-info.component.css']
})
export class GenreInfoComponent {
  @Input()
  genre!: FullGenreModel;
  @Input()
  isAdmin = false;
  isUpdate = false;

  constructor(
    private genresService: GenresService,
    private authService: AuthService,
    private router: Router
    ) {}

  ngOnInit(): void {
    this.isAdminCheck();
  }

  deleteGenre(genreId: number): void {
    this.genresService.deleteGenre(genreId).subscribe({
      next: () => { 
          Swal.fire({
          position: "bottom-end",
          icon: "success",
          title: "Deleted",
          showConfirmButton: false,
          timer: 2500
        });
        this.router.navigate(['/books-filters']);
        return;
      },
      error: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "error",
          title: "Error deleting. Make sure you`ve deleted all books with this genre before",
          showConfirmButton: false,
          timer: 2500
        });
        return;
      }
    });
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
