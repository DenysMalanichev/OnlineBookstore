import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { CommentModel } from '../models/comments-models/commentModel';
import { environment } from 'src/environments/environment.development';
import { GetCommentModel } from '../models/comments-models/getCommentModel';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  commentsBasePath = environment.apiBaseUrl + environment.endpoints.comments.commentsBasePath;

  constructor(private http: HttpClient) { }

  createComment(comment: CommentModel) {
    return this.http.post(this.commentsBasePath, { ...comment });
  }

  getCommentsByBook(bookId: number): Observable<GetCommentModel[]> {
    const commentsPath = this.commentsBasePath + environment.endpoints.comments.getCommentsByBookPath;
    let params = new HttpParams().set('bookId', bookId);

    return this.http.get<GetCommentModel[]>(commentsPath, { params });
  }

  deleteComment(commentId: number) {
    const commentsPath = this.commentsBasePath;
    let params = new HttpParams().set('commentId', commentId);

    return this.http.delete(commentsPath, { params });
  }
}
