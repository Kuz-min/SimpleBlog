import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { debounce, interval, Observable } from 'rxjs';
import { AuthorizationService, ServerApiConstants } from 'simple-blog/core';

export const isAuthorizedToEditPostTagsGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> => {
  return inject(AuthorizationService).isAuthorizedToEditPostTags().pipe(
    debounce(isAuthorized => isAuthorized ? interval(0) : interval(ServerApiConstants.DefaultHttpTimeout)),
  );
};
