import { Injectable } from '@angular/core';
import { combineLatest, map, Observable } from 'rxjs';
import { Post, ProfileService } from 'simple-blog/core';

@Injectable()
export class AuthorizationService {

  constructor(
    private readonly _profileService: ProfileService,
  ) { }

  public isAuthorizedToEditPost(post: Observable<Post | null>): Observable<boolean> {
    return combineLatest(this._profileService.getCurrentAsync(), post).pipe(
      map(([profile, post]) => profile && post && post.ownerId == profile.id ? true : false),
    );
  }

}
