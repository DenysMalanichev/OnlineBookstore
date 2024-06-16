import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CommentModel } from 'src/app/models/comments-models/commentModel';
import { CommentsService } from 'src/app/services/comments.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'comments-container',
  templateUrl: './comments-container.component.html',
  styleUrls: ['./comments-container.component.css']
})
export class CommentsContainerComponent implements OnInit {
  @Input()
  bookId!: number;
  
  newComment = new FormGroup({
    title: new FormControl(''),
    body: new FormControl(''),
    bookRating: new FormControl(0)
  });

  comments!: CommentModel[];

  constructor(private commentsService: CommentsService) {}

  ngOnInit(): void {
    this.getCommentsByBook(this.bookId);
  }

  writeComment(): void {
    console.log(this.newComment.get('rating')?.value)
    this.commentsService.createComment(this.generateCommentModel(this.newComment.value))
    .subscribe({
      next: () => {
        this.getCommentsByBook(this.bookId);
      },
      error: () => Swal.fire({
        position: "top-end",
        icon: "error",
        title: "An error occured. Maybe you\'ve already wrote a review",
        showConfirmButton: false,
        timer: 2500
      })
    });
  }

  getCommentsByBook(bookId: number): void {
    this.commentsService.getCommentsByBook(bookId).subscribe(x => this.comments = x);
  }

  private generateCommentModel(formValue: any): CommentModel {
    return {
      title: formValue.title ?? '',
      body: formValue.body ?? '',
      bookRating: formValue.bookRating ?? 0,
      bookId: this.bookId
    };
  }
}
