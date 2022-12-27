import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, filter, map, mergeMap, Observable, of, switchMap, throwError } from 'rxjs';
import { Post, PostService, ProfileService } from 'simple-blog/core';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostComponent implements OnInit {

  post: (Observable<Post | null> | null) = null;

  isAuthorizedToEdit: (Observable<boolean> | null) = null;

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _postService: PostService,
    private readonly _profileService: ProfileService,
  ) { }

  ngOnInit(): void {

    this.post = this._route.params.pipe(
      filter(params => params['id']),
      map(params => Number(params['id'])),
      mergeMap(id => this._postService.getByIdAsync(id).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of(null) : throwError(error)),
      )),
    );

    //move to authorization services
    this.isAuthorizedToEdit = this._profileService.getCurrentAsync().pipe(
      switchMap(profile => this.post!.pipe(
        map(post => post && profile && post.ownerId == profile.id ? true : false),
      )),
    );

  }

}
