import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { BehaviorSubject, Observable, of, switchMap } from 'rxjs';
import { catchNotFound, filterNotNullAndNotUndefined, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-tag-preview',
  templateUrl: './post-tag-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostTagPreviewComponent {

  @Input() set tagId(id: number | null | undefined) {
    this._id.next(id);
  }

  readonly tag: Observable<PostTag | null>;

  constructor(
    private readonly _postTagService: PostTagService,
  ) {

    this.tag = this._id.pipe(
      filterNotNullAndNotUndefined(),
      switchMap(id => this._postTagService.getByIdAsync(id).pipe(
        catchNotFound(of(null)),
      )),
    );

  }

  private readonly _id = new BehaviorSubject<number | null | undefined>(null);

}
