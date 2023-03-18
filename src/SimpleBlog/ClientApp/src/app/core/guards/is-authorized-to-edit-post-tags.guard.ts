import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { audit, interval, Observable } from 'rxjs';
import { AuthorizationService, ServerApiConstants } from 'simple-blog/core';

export const isAuthorizedToEditPostTagsGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> => {
  return inject(AuthorizationService).isAuthorizedToEditPostTags().pipe(
    audit(isAuthorized => isAuthorized ? interval(0) : interval(ServerApiConstants.DefaultHttpTimeout)),
  );
};
