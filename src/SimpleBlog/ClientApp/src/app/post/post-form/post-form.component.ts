import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { catchError, filter, Observable, of, throwError } from 'rxjs';
import { Post, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-form',
  templateUrl: './post-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostFormComponent implements OnInit {

  @Input() post: (Observable<Post | null> | null | undefined);
  @Input() disabled: (Observable<boolean> | null | undefined);
  @Output() readonly complited = new EventEmitter<{ title: string, content: string, tagIds?: number[] }>();

  tags: (Observable<PostTag[] | null> | null) = null;

  readonly form = new FormGroup({
    title: new FormControl(''),
    content: new FormControl(''),
    tagIds: new FormControl<number[]>([]),
  });

  constructor(
    private readonly _postTagService: PostTagService,
  ) { }

  ngOnInit(): void {
    this.tags = this._postTagService.getAllAsync().pipe(
      catchError(error => (error as HttpErrorResponse)?.status == 404 ? of([]) : throwError(error)),
    );

    if (this.post) {
      this.post.pipe(
        filter(post => Boolean(post)),
      ).subscribe(
        next => this.form.patchValue(next!),
      );
    }

    if (this.disabled) {
      this.disabled.subscribe(
        next => next ? this.form.disable() : this.form.enable(),
      );
    }

  }

  onSubmit(): void {
    let data = this.form.value;
    this.complited.emit({ title: data.title!, content: data.content!, tagIds: data.tagIds! });
  }

}
