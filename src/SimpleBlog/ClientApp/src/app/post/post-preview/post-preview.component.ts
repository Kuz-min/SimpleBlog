import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { BehaviorSubject, Observable, of, switchMap } from 'rxjs';
import { catchNotFound, filterNotNullAndNotUndefined, Post, PostService } from 'simple-blog/core';

@Component({
  selector: 'post-preview',
  templateUrl: './post-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostPreviewComponent {

  @Input() set postId(id: number | null | undefined) {
    this._id.next(id);
  }

  readonly post: Observable<Post | null>;

  constructor(
    private readonly _postService: PostService,
  ) {

    this.post = this._id.pipe(
      filterNotNullAndNotUndefined(),
      switchMap(id => this._postService.getByIdAsync(id).pipe(
        catchNotFound(of(null)),
      )),
    );

  }

  private readonly _id = new BehaviorSubject<number | null | undefined>(null);

}
