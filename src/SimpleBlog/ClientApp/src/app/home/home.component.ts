import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { catchError, map, mergeMap, Observable, of, tap, throwError } from 'rxjs';
import { Post, PostService, PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent implements OnInit {

  tags: (Observable<PostTag[] | null> | null) = null;
  posts: (Observable<Post[] | null> | null) = null;
  selectedTags: number[] = [];

  constructor(
    private readonly _router: Router,
    private readonly _route: ActivatedRoute,
    private readonly _postService: PostService,
    private readonly _postTagService: PostTagService,
  ) { }

  ngOnInit(): void {
    this.tags = this._postTagService.getAllAsync().pipe(
      catchError(error => (error as HttpErrorResponse)?.status == 404 ? of([]) : throwError(error)),
    );

    this.posts = this._route.queryParams.pipe(
      map(params => params['tag_ids']?.split(',')?.map((o: string) => Number(o)) || []),
      tap(tagIds => this.selectedTags = tagIds),
      mergeMap(tagIds => this._postService.searchAsync(tagIds).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of([]) : throwError(error)),
      )),
    );
  }

  applyFilter(): void {
    let params: Params = [];

    if (this.selectedTags?.length > 0)
      params['tag_ids'] = this.selectedTags.join(',');

    this._router.navigate([],
      {
        relativeTo: this._route,
        queryParams: params,
        //queryParamsHandling: 'merge', //remove to replace all query params by provided
      });
  }

}
