import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { audit, interval, Observable } from 'rxjs';
import { AuthenticationService } from 'simple-blog/core';

@Injectable()
export class IsAuthenticatedGuard implements CanActivate {

  constructor(
    private readonly _authService: AuthenticationService
  ) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this._authService.isAuthenticatedAsync().pipe(
      audit(isAuthenticated => isAuthenticated ? interval(0) : interval(250)),
    );
  }

}
