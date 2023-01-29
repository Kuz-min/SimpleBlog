import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { ErrorRequestResult } from '@ngneat/elf-requests/src/lib/requests-result';
import { catchError, EMPTY, filter, first, Observable, of, switchMap, throwError, timeout } from 'rxjs';
import { Profile } from 'simple-blog/core';

@Injectable()
export class ProfileService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: string): Observable<Profile> {
    const key = ['profile', id];

    getRequestResult(key, { initialStatus: 'idle' }).pipe(
      first(),
      filter(request => !(request.isLoading)),
      switchMap(() => this._http.get<Profile>(this._urls.getById(id)).pipe(
        first(),
        timeout(3000),
        trackRequestResult(key, { staleTime: 30_000 }),
      )),
      catchError(() => EMPTY),
    ).subscribe(
      next => this._profileStore.update(upsertEntities(next)),
    );

    return this._profileStore.pipe(
      selectEntity(id),
      joinRequestResult(key),
      filter(request => request.status != 'idle' && request.status != 'loading'),
      switchMap(request => request.isSuccess ? of(request.data as Profile) : throwError((request as ErrorRequestResult).error)),
    );
  }

  private readonly _profileStore = createStore(
    { name: 'profile-store' },
    withEntities<Profile>(),
  );

  private readonly _urls = {
    getById: (id: string) => `${this._baseUrl}api/profiles/${id}`,
  }
}
