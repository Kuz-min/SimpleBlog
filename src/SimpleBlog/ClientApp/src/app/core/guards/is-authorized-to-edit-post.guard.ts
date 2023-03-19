import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { catchError, debounce, interval, Observable, of } from 'rxjs';
import { AuthorizationService, PostService, ServerApiConstants } from 'simple-blog/core';

export const isAuthorizedToEditPostGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> => {
  const id = route.params['id'];

  const post = id ? inject(PostService).getByIdAsync(id).pipe(catchError(() => of(null))) : of(null);

  return inject(AuthorizationService).isAuthorizedToEditPost(post).pipe(
    debounce(isAuthorized => isAuthorized ? interval(0) : interval(ServerApiConstants.DefaultHttpTimeout)),
  );
};
