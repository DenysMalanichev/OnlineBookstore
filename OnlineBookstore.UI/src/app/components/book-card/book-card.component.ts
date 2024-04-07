import { Component, Input } from '@angular/core';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';

@Component({
  selector: 'book-card',
  templateUrl: './book-card.component.html',
  styleUrls: ['./book-card.component.css']
})
export class BookCardComponent {
  @Input()
  book!: BriefBookModel;
}
