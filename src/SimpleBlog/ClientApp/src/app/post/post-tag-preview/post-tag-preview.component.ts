import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';
import { PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-tag-preview',
  templateUrl: './post-tag-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostTagPreviewComponent implements OnInit {

  @Input() tagId: (number | null | undefined);

  tag: (Observable<PostTag | null> | null) = null;

  constructor(
    private readonly _postTagService: PostTagService,
  ) { }

  ngOnInit(): void {
    if (this.tagId) {
      this.tag = this._postTagService.getByIdAsync(this.tagId).pipe(
        catchError(error => (error instanceof HttpErrorResponse) && error.status == 404 ? of(null) : throwError(error)),
      );
    }
  }

}
