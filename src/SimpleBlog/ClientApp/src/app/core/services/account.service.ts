import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { first, Observable, shareReplay, timeout } from 'rxjs';

@Injectable()
export class AccountService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient
  ) { }

  public createAsync(data: { username: string, email: string, password: string }): Observable<any> {
    return this._http.post(this._baseUrl + 'api/accounts', data).pipe(
      first(),
      timeout(3000),
      shareReplay(),
    );
  }

}
