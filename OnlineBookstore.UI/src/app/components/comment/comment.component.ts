import { Component, Input } from '@angular/core';
import { CommentModel } from 'src/app/models/comments-models/commentModel';

@Component({
  selector: 'comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent {
  @Input()
  comment!: CommentModel;
}
