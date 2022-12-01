import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { filter, first, mergeMap, Observable, timeout } from 'rxjs';
import { AuthenticationService } from 'simple-blog/core';

@Injectable()
export class AddTokenInterceptor implements HttpInterceptor {

  constructor(
    private readonly _injector: Injector
  ) { }

  private get authService() { return this._injector.get(AuthenticationService); }

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    if (!request.url.includes(`${window.location.protocol}//${window.location.host}/`))
      return next.handle(request);

    if (request.headers.has('Authorization')) {
      return this.authService.getTokenAsync().pipe(
        filter(token => Boolean(token)),//not null, empty, etc
        first(),
        timeout(3000),
        mergeMap(token => {
          let newRequest = request.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
          return next.handle(newRequest);
        }),
      );
    }

    return next.handle(request);
  }

}
