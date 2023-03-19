import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { MatSnackBar, MatSnackBarDismiss } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, catchError, filter, finalize, first, map, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { Post, PostFormModel, PostService } from 'simple-blog/core';
import { PostChangedAlertComponent } from '../post-changed-alert/post-changed-alert.component';

@Component({
  selector: 'post-editor',
  templateUrl: './post-editor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostEditorComponent implements OnInit {

  post: (Observable<Post | null> | null) = null;

  readonly lockForm = new BehaviorSubject<boolean>(false);

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _snackBar: MatSnackBar,
    private readonly _postService: PostService,
  ) { }

  ngOnInit(): void {
    this.post = this._route.params.pipe(
      filter(params => Boolean(params['id'])),
      map(params => Number(params['id'])),
      tap(id => this._postId = id),
      switchMap(id => this._postService.getByIdAsync(id).pipe(
        first(),
        catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of(null) : throwError(error)),
      )),
    );
  }

  updatePost(data: PostFormModel): void {
    if (!this._postId)
      return;

    this.lockForm.next(true);
    this._postService.updateAsync(this._postId, data).pipe(
      first(),
      switchMap(post => this.showAlert('Post updated!', post)),
      catchError(error => error instanceof HttpErrorResponse ? this.showAlert(`Error! ${error.message}`) : throwError(error)),
      finalize(() => this.lockForm.next(false)),
    ).subscribe();
  }

  deletePost(): void {
    if (!this._postId)
      return;

    this.lockForm.next(true);
    this._postService.deleteAsync(this._postId).pipe(
      first(),
      switchMap(() => this.showAlert('Post deleted!')),
      catchError(error => error instanceof HttpErrorResponse ? this.showAlert(`Error! ${error.message}`) : throwError(error)),
      finalize(() => this.lockForm.next(false)),
    ).subscribe();
  }

  private showAlert(message: string, post?: Post): Observable<MatSnackBarDismiss> {
    return this._snackBar.openFromComponent(PostChangedAlertComponent, { verticalPosition: 'top', data: { message: message, post: post } }).afterDismissed();
  }

  private _postId: (number | null) = null;

}
