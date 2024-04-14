import { Component, OnInit } from '@angular/core';
import { FullGenreModel } from 'src/app/models/genre-models/fullGenreModel';
import { GenresService } from 'src/app/services/genres-service.service';

@Component({
  selector: 'app-genres-list',
  templateUrl: './genres-list.component.html',
  styleUrls: ['./genres-list.component.css']
})
export class GenresListComponent implements OnInit {
  genres!: FullGenreModel[];

  constructor(private genresService: GenresService) {}

  ngOnInit(): void {
    this.getGenres(); 
  }

  getGenres(): void {
    this.genresService.getAllGenres().subscribe(x => {this.genres = x; console.log(this.genres);});
  }
}
