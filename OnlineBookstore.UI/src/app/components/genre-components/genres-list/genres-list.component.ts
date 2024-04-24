import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FullGenreModel } from 'src/app/models/genre-models/fullGenreModel';
import { AuthService } from 'src/app/services/auth.service';
import { GenresService } from 'src/app/services/genres-service.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'genres-list',
  templateUrl: './genres-list.component.html',
  styleUrls: ['./genres-list.component.css']
})
export class GenresListComponent implements OnInit {
  genres!: FullGenreModel[];
  isAdmin = false;
  isCreate = false;

  constructor(
    private genresService: GenresService,
    private authService: AuthService
    ) {}

  ngOnInit(): void {
    this.getGenres(); 
    this.isAdminCheck();
  }

  getGenres(): void {
    this.genresService.getAllGenres().subscribe(x => this.genres = x);
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
