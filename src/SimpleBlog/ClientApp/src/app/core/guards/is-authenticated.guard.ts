import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { audit, interval, Observable } from 'rxjs';
import { AuthenticationService, ServerApiConstants } from 'simple-blog/core';

export const isAuthenticatedGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> => {
  return inject(AuthenticationService).isAuthenticatedAsync().pipe(
    audit(isAuthenticated => isAuthenticated ? interval(0) : interval(ServerApiConstants.DefaultHttpTimeout)),
  );
};
