import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, filter, map, mergeMap, Observable, of, throwError } from 'rxjs';
import { Post, PostService } from 'simple-blog/core';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostComponent implements OnInit {

  post: (Observable<Post | null> | null) = null;

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _postService: PostService,
  ) { }

  ngOnInit(): void {
    this.post = this._route.params.pipe(
      filter(params => params['id']),
      map(params => Number(params['id'])),
      mergeMap(id => this._postService.getByIdAsync(id).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of(null) : throwError(error)),
      )),
    );
  }

}
