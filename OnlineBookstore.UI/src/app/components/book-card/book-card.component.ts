import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { BriefBookModel } from 'src/app/models/book-models/briefBookModel';
import { BooksService } from 'src/app/services/books-service.service';

@Component({
  selector: 'book-card',
  templateUrl: './book-card.component.html',
  styleUrls: ['./book-card.component.css']
})
export class BookCardComponent implements OnInit {
  @Input()
  book!: BriefBookModel;
  bookImageUrl!: SafeUrl;

  constructor(
      private booksService: BooksService,
      private sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.booksService.getBookImage(this.book.id).subscribe(i => {
      if (i && i.size > 0) {
        const imageObjectUrl = URL.createObjectURL(i);
        this.bookImageUrl = this.sanitizer.bypassSecurityTrustUrl(imageObjectUrl);
      } else {
        this.bookImageUrl = 'https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/660px-No-Image-Placeholder.svg.png?20200912122019';
      }
    });
  }
}
