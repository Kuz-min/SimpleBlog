import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';
import { Post, PostService } from 'simple-blog/core';

@Component({
  selector: 'post-preview',
  templateUrl: './post-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostPreviewComponent implements OnInit {

  @Input() postId: (number | null | undefined);

  post: (Observable<Post | null> | null) = null;

  constructor(
    private readonly _postService: PostService,
  ) { }

  ngOnInit(): void {

    if (this.postId)
      this.post = this._postService.getByIdAsync(this.postId).pipe(
        catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of(null) : throwError(() => error)),
      );

  }

}
