import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { audit, interval, Observable } from 'rxjs';
import { AuthorizationService, PostService } from 'simple-blog/core';

@Injectable()
export class IsAuthorizedToEditPostGuard implements CanActivate {

  constructor(
    private readonly _authorizationService: AuthorizationService,
    private readonly _postService: PostService,
  ) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const id = route.params['id'];
    const post = this._postService.getByIdAsync(id);

    return this._authorizationService.isAuthorizedToEditPost(post).pipe(
      audit(isAuthorized => isAuthorized ? interval(0) : interval(250)),
    );
  }

}
