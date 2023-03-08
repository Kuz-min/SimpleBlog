import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, catchError, filter, first, Observable, of, Subscription, throwError } from 'rxjs';
import { Post, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-form',
  templateUrl: './post-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostFormComponent implements OnInit, OnDestroy {

  @Input() post: (Observable<Post | null> | null | undefined);
  @Input() disabled: (Observable<boolean> | null | undefined);
  @Output() readonly complited = new EventEmitter<{ title: string, content: string, image?: File, tagIds?: number[] }>();

  tags: (Observable<PostTag[] | null> | null) = null;

  readonly imagePath = new BehaviorSubject<string | null>(null);

  readonly form = new FormGroup({
    title: new FormControl('', [Validators.required]),
    content: new FormControl('', [Validators.required]),
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
      this._postSubscription = this.post.pipe(
        filter(post => Boolean(post)),
        first(),
      ).subscribe(
        next => this.form.patchValue(next!),
      );
    }

    if (this.disabled) {
      this._disabledSubscription = this.disabled.subscribe(
        next => next ? this.form.disable() : this.form.enable(),
      );
    }

  }

  ngOnDestroy(): void {
    this._postSubscription?.unsubscribe();
    this._disabledSubscription?.unsubscribe();
  }

  onSubmit(): void {
    const data = this.form.value;
    this.complited.emit({ title: data.title!, content: data.content!, image: this._imageFile!, tagIds: data.tagIds! });
  }

  imagePreview(event: any): void {
    const file = (event.target as HTMLInputElement)?.files?.[0];

    if (!file)
      return;

    this._imageFile = file;

    const reader = new FileReader();
    reader.onload = () => this.imagePath.next(reader.result as string);
    reader.readAsDataURL(file);
  }

  private _imageFile: (File | null) = null;
  private _postSubscription: (Subscription | null) = null;
  private _disabledSubscription: (Subscription | null) = null;

}
