import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { selectAllEntities, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { catchError, EMPTY, filter, first, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { AccountRole, ServerApiConstants } from 'simple-blog/core';

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
        tap(roles => this._accountRoleStore.update(upsertEntities(roles))),
        trackRequestResult(key, { staleTime: ServerApiConstants.DefaultCacheTimeout }),
      )),
      catchError(() => EMPTY),
    ).subscribe();

    return this._accountRoleStore.pipe(
      selectAllEntities(),
      joinRequestResult(key),
      filter(request => request.isSuccess || request.isError),
      switchMap(request => request.isSuccess ? of(request.data) : request.isError ? throwError(() => request.error) : EMPTY),
    );
  }

  private readonly _accountRoleStore = createStore(
    { name: 'account-role-store' },
    withEntities<AccountRole, 'name'>({ idKey: 'name' }),
  );

  private readonly _urls = {
    getAll: () => `${this._baseUrl}api/account-roles`,
  };

}
