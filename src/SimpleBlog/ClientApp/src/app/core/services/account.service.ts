import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { first, Observable, shareReplay } from 'rxjs';

@Injectable()
export class AccountService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient
  ) { }

  public createAsync(data: { username: string, email: string, password: string }): Observable<any> {
    return this._http.post(this._baseUrl + 'api/accounts', data).pipe(
      first(),
      shareReplay(),
    );
  }

  public updatePassword(data: { currentPassword: string, newPassword: string }): Observable<any> {
    return this._http.put(this._baseUrl + 'api/accounts/password', data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      shareReplay(),
    );
  }

}
