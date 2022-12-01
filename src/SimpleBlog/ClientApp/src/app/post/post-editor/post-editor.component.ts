import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, catchError, filter, map, mergeMap, Observable, of, tap, throwError } from 'rxjs';
import { Post, PostService } from 'simple-blog/core';

@Component({
  selector: 'post-editor',
  templateUrl: './post-editor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostEditorComponent implements OnInit {

  post: (Observable<Post | null> | null) = null;
  lockForm = new BehaviorSubject<boolean>(false);
  isUpdated = new BehaviorSubject<boolean>(false);
  isDeleted = new BehaviorSubject<boolean>(false);

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _postService: PostService,
  ) { }

  ngOnInit(): void {
    this.post = this._route.params.pipe(
      filter(params => params['id']),
      map(params => Number(params['id'])),
      tap(id => this._postId = id),
      mergeMap(id => this._postService.getByIdAsync(id).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of(null) : throwError(error)),
      )),
    );
  }

  updatePost(data: { title: string, content: string, tagIds?: number[] }): void {
    this.lockForm.next(true);
    this._postService.updateAsync(this._postId!, { title: data.title, content: data.content, tagIds: data.tagIds }).subscribe(
      next => {
        this.lockForm.next(false);
        this.isUpdated.next(true);
      },
      error => {
        this.lockForm.next(false);
      },
    );
  }

  deletePost(): void {
    this._postService.deleteAsync(this._postId!).subscribe(
      next => {
        this.isDeleted.next(true);
      },
    );
  }

  private _postId: (number | null) = null;

}
