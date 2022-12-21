import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { BehaviorSubject, defer, Observable, tap } from 'rxjs';

@Injectable()
export class AuthenticationService {

  constructor(private readonly _oAuthService: OAuthService) {
    //move config to another place
    this._oAuthService.issuer = `${window.location.protocol}//${window.location.host}/`;//https://localhost:44468/
    this._oAuthService.clientId = 'simple-blog-web-client';
    this._oAuthService.dummyClientSecret = 'simple-blog-web-client';
    this._oAuthService.scope = "offline_access";
    this._oAuthService.responseType = 'token';
    this._oAuthService.oidc = false;

    this._oAuthService.loadDiscoveryDocument()
      .then(() => { this._isReady = true; this.checkAndUpdateToken(); })
      .catch((reason) => console.error(reason));
  }

  public isAuthenticatedAsync(): Observable<boolean> {
    this.checkAndUpdateToken();
    return this._isAuthenticated.asObservable();
  }

  public getTokenAsync(): Observable<string | null> {
    this.checkAndUpdateToken();
    return this._token.asObservable();
  }

  public signInAsync(data: { username: string, password: string }): Observable<any> {
    return defer(() => this._oAuthService.fetchTokenUsingGrant("password", data)).pipe(
      tap(responce => {
        this._token.next(responce.access_token);
        this._isAuthenticated.next(true);
      }),
    );
  }

  private checkAndUpdateToken(): void {
    if (!this._isReady || this._isUpdating)
      return;

    if (this._oAuthService.hasValidAccessToken() && this._isAuthenticated.getValue() && this._token.getValue())
      return;

    if (!this._oAuthService.getRefreshToken())
      return;

    this._isUpdating = true;

    this._oAuthService.refreshToken()
      .then(responce => {
        this._token.next(responce.access_token);
        this._isAuthenticated.next(true);
      })
      .catch(error => {
        this._token.next(null);
        this._isAuthenticated.next(false);
      })
      .finally(() => {
        this._isUpdating = false;
      });
  }

  private _isReady = false;
  private _isUpdating = false;
  private readonly _isAuthenticated = new BehaviorSubject(false);
  private readonly _token = new BehaviorSubject<string | null>(null);
}
