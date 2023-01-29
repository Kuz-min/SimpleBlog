import { Injectable } from '@angular/core';
import { combineLatest, map, Observable } from 'rxjs';
import { AuthenticationService, Post } from 'simple-blog/core';

@Injectable()
export class AuthorizationService {

  constructor(
    private readonly _authService: AuthenticationService,
  ) { }

  public isAuthorizedToEditPost(post: Observable<Post | null>): Observable<boolean> {
    return combineLatest(this._authService.getIdAsync(), post).pipe(
      map(([id, post]) => id && post && post.ownerId == id ? true : false),
    );
  }

}
