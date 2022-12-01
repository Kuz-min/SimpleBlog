import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { BehaviorSubject, catchError, of, throwError } from 'rxjs';
import { PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-tag-editor',
  templateUrl: './post-tag-editor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostTagEditorComponent implements OnInit {

  readonly tags = new BehaviorSubject<PostTag[] | null>(null);

  constructor(
    private readonly _postTagService: PostTagService,
  ) { }

  ngOnInit(): void {
    this._postTagService.getAllAsync().pipe(
      catchError(error => (error as HttpErrorResponse)?.status == 404 ? of([]) : throwError(error)),
    ).subscribe(
      next => this.tags.next(next),
    );
  }

  createTag(input: HTMLInputElement): void {
    this._postTagService.createAsync({ title: input.value }).subscribe(
      next => {
        this.tags.next(this.tags.value ? [...this.tags.value!, next!] : [next!]);
        input.value = '';
      },
    );
  }

  updateTag(tag: PostTag, title: string): void {
    this._postTagService.updateAsync(tag.id, { title: title }).subscribe();
  }

  deleteTag(tag: PostTag): void {
    this._postTagService.deleteAsync(tag.id).subscribe(
      next => this.tags.next(this.tags.value?.filter(o => o != tag)!),
    );
  }

}
