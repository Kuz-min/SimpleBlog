import { ChangeDetectionStrategy, Component } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { map, Observable, of, switchMap, tap } from 'rxjs';
import { catchNotFound, PostService, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent {

  selectedTags: (number[] | null) = null;
  pageIndex: number = 0;
  length: number = 0;

  readonly tags: Observable<PostTag[]>;
  readonly postIds: Observable<number[]>;

  constructor(
    private readonly _router: Router,
    private readonly _route: ActivatedRoute,
    private readonly _postService: PostService,
    private readonly _postTagService: PostTagService,
  ) {

    this.tags = this._postTagService.getAllAsync();

    this.postIds = this._route.queryParams.pipe(
      map(params => ({
        index: Number(params['page']) ?? 0,
        tagIds: params['tag_ids']?.split(',')?.map((o: string) => Number(o)) ?? null
      })),
      tap(params => {
        this.pageIndex = params.index;
        this.selectedTags = params.tagIds;
      }),
      switchMap(params => this._postService.searchAsync(params.tagIds, params.index * 10, 10).pipe(
        tap({
          next: list => this.length = list.length,
          error: () => this.length = 0,
        }),
        map(list => list.items),
        catchNotFound(of([])),
      )),
    );

  }

  applyFilter(): void {
    this.pageIndex = 0;
    this.navigate();
  }

  handlePaginatorEvent(e: PageEvent): void {
    this.pageIndex = e.pageIndex;
    this.navigate();
  }

  private navigate(): void {
    const params: Params = [];

    if (this.selectedTags && this.selectedTags.length > 0)
      params['tag_ids'] = this.selectedTags.slice().sort((a, b) => a - b).join(',');

    if (this.pageIndex > 0)
      params['page'] = this.pageIndex;

    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: params,
      //queryParamsHandling: 'merge', //remove to replace all query params by provided
    });
  }

}
