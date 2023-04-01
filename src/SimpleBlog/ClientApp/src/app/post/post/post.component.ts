import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, filter, map, Observable, of, switchMap, throwError } from 'rxjs';
import { AuthorizationService, Post, PostService } from 'simple-blog/core';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostComponent {

  readonly post: Observable<Post | null>;
  readonly isAuthorizedToEdit: Observable<boolean>;

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _authorizationService: AuthorizationService,
    private readonly _postService: PostService,
  ) {

    this.post = this._route.params.pipe(
      map(params => Number(params['id'])),
      filter(id => !Number.isNaN(id)),
      switchMap(id => this._postService.getByIdAsync(id).pipe(
        catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of(null) : throwError(() => error)),
      )),
    );

    this.isAuthorizedToEdit = this._authorizationService.isAuthorizedToEditPost(this.post);

  }

}
