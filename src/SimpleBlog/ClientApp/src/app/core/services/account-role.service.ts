import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { selectAllEntities, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { ErrorRequestResult } from '@ngneat/elf-requests/src/lib/requests-result';
import { catchError, EMPTY, filter, first, Observable, of, switchMap, throwError, timeout } from 'rxjs';
import { AccountRole } from 'simple-blog/core';

@Injectable()
export class AccountRoleService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getAllAsync(): Observable<AccountRole[]> {
    const key = ['account-role-all'];

    getRequestResult(key, { initialStatus: 'idle' }).pipe(
      first(),
      filter(request => !(request.isLoading)),
      switchMap(() => this._http.get<AccountRole[]>(this._urls.getAll()).pipe(
        first(),
        timeout(3000),
        catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of([]) : throwError(error)),
        trackRequestResult(key, { staleTime: 30_000 }),
      )),
      catchError(() => EMPTY),
    ).subscribe(
      next => this._accountRoleStore.update(upsertEntities(next)),
    );

    return this._accountRoleStore.pipe(
      selectAllEntities(),
      joinRequestResult(key),
      filter(request => request.status != 'idle' && request.status != 'loading'),
      switchMap(request => request.isSuccess ? of(request.data as AccountRole[]) : throwError((request as ErrorRequestResult).error)),
    );
  }

  private readonly _accountRoleStore = createStore(
    { name: 'account-role-store' },
    withEntities<AccountRole, 'name'>(),
  );

  private readonly _urls = {
    getAll: () => `${this._baseUrl}api/account-roles`,
  };

}
