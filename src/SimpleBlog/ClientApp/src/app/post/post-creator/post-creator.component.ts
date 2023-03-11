import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BehaviorSubject, catchError, finalize, first, switchMap, throwError } from 'rxjs';
import { PostService } from 'simple-blog/core';
import { PostChangedAlertComponent } from '../post-changed-alert/post-changed-alert.component';

@Component({
  selector: 'post-creator',
  templateUrl: './post-creator.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostCreatorComponent {

  readonly lockForm = new BehaviorSubject<boolean>(false);

  constructor(
    private readonly _snackBar: MatSnackBar,
    private readonly _postService: PostService,
  ) { }

  onComplited(data: { title: string, content: string, image?: File, tagIds?: number[] }): void {
    this.lockForm.next(true);
    this._postService.createAsync(data).pipe(
      first(),
      switchMap(post => this._snackBar.openFromComponent(PostChangedAlertComponent, { verticalPosition: 'top', data: { message: 'Post created!', post: post } }).afterDismissed()),
      catchError(error => error instanceof HttpErrorResponse ?
        this._snackBar.openFromComponent(PostChangedAlertComponent, { verticalPosition: 'top', data: { message: `Error! ${error.message}` } }).afterDismissed()
        : throwError(error)
      ),
      finalize(() => this.lockForm.next(false)),
    ).subscribe();
  }

}
