import { Component, Input, OnInit } from '@angular/core';
import { CommentModel } from 'src/app/models/comments-models/commentModel';
import { GetCommentModel } from 'src/app/models/comments-models/getCommentModel';
import { AuthService } from 'src/app/services/auth.service';
import { CommentsService } from 'src/app/services/comments.service';

@Component({
  selector: 'comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnInit {
  @Input()
  comment!: GetCommentModel;
  isAdmin = false;

  constructor(
    private authService: AuthService,
    private commentsService: CommentsService
  ) {}

  ngOnInit(): void {
    this.isAdminCheck();
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }

  deleteComment(): void {
    this.commentsService.deleteComment(this.comment.id).subscribe(() => window.location.reload());
  }
}
