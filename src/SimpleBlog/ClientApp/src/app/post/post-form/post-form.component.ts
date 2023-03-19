import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, catchError, delay, filter, first, Observable, of, Subject, takeUntil, tap, throwError } from 'rxjs';
import { Post, PostFormModel, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-form',
  templateUrl: './post-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostFormComponent implements OnInit, OnDestroy {

  @Input() post: (Observable<Post | null> | null | undefined);
  @Input() disabled: (Observable<boolean> | null | undefined);
  @Output() readonly complited = new EventEmitter<PostFormModel>();

  readonly tags: Observable<PostTag[]>;
  readonly imagePath = new BehaviorSubject<string | null>(null);
  readonly form = new FormGroup({
    title: new FormControl('', [Validators.required]),
    content: new FormControl('', [Validators.required]),
    image: new FormControl(),
    tagIds: new FormControl<number[]>([]),
  });

  constructor(
    private readonly _changeDetector: ChangeDetectorRef,
    private readonly _postTagService: PostTagService,
  ) {

    this.tags = this._postTagService.getAllAsync().pipe(
      catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of([]) : throwError(() => error)),
    );

  }

  ngOnInit(): void {

    if (this.post) {
      this.post.pipe(
        takeUntil(this._isDestroyed),
        filter(post => Boolean(post)),
        first(),
        tap(post => this.form.patchValue({ ...post, image: null }))
      ).subscribe();
    }

    if (this.disabled) {
      this.disabled.pipe(
        takeUntil(this._isDestroyed),
        delay(1),
        tap(value => {
          if (value)
            this.form.disable();
          else
            this.form.enable();
          this._changeDetector.markForCheck();
        }),
      ).subscribe();
    }

  }

  ngOnDestroy(): void {
    this._isDestroyed.next(true);
    this._isDestroyed.complete();
  }

  onSubmit(): void {
    const data = { ...this.form.value, image: this._imageFile } as PostFormModel;
    this.complited.emit(data);
  }

  imagePreview(event: any): void {
    const file = (event.target as HTMLInputElement)?.files?.[0];

    if (file) {
      this._imageFile = file;
      const reader = new FileReader();
      reader.onload = () => this.imagePath.next(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  private _imageFile: (File | null) = null;
  private readonly _isDestroyed = new Subject<boolean>();

}
