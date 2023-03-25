import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { catchError, map, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { PostService, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent implements OnInit {

  selectedTags: (number[] | null) = null;

  readonly tags: Observable<PostTag[]>;
  readonly postIds: Observable<number[]>;

  constructor(
    private readonly _router: Router,
    private readonly _route: ActivatedRoute,
    private readonly _postService: PostService,
    private readonly _postTagService: PostTagService,
  ) {

    this.tags = this._postTagService.getAllAsync().pipe(
      catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of([]) : throwError(() => error)),
    );

    this.postIds = this._route.queryParams.pipe(
      map(params => params['tag_ids']?.split(',')?.map((o: string) => Number(o)) || null),
      tap(tagIds => this.selectedTags = tagIds),
      switchMap(tagIds => this._postService.searchAsync(tagIds).pipe(
        catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of([]) : throwError(() => error)),
      )),
    );

  }

  ngOnInit(): void {
  }

  applyFilter(): void {
    const params: Params = [];

    if (this.selectedTags && this.selectedTags.length > 0)
      params['tag_ids'] = this.selectedTags.slice().sort((a, b) => a - b).join(',');

    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: params,
      //queryParamsHandling: 'merge', //remove to replace all query params by provided
    });
  }

}
