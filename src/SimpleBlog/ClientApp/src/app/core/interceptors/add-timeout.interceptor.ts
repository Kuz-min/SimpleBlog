import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError, timeout } from 'rxjs';
import { ServerApiConstants } from 'simple-blog/core';

@Injectable()
export class AddTimeoutInterceptor implements HttpInterceptor {

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      timeout({
        first: ServerApiConstants.DefaultHttpTimeout,
        with: () => throwError(() => new HttpErrorResponse({ error: 'Request timeout expired', status: 504 }))
      }),
    );
  }

}
