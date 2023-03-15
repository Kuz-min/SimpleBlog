import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { audit, interval, Observable } from 'rxjs';
import { AuthorizationService } from 'simple-blog/core';

@Injectable()
export class IsAuthorizedToEditPostTagsGuard implements CanActivate {

  constructor(
    private readonly _authorizationService: AuthorizationService,
  ) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this._authorizationService.isAuthorizedToEditPostTags().pipe(
      audit(isAuthorized => isAuthorized ? interval(0) : interval(250)),
    );
  }

}
