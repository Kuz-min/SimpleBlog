import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { ErrorRequestResult } from '@ngneat/elf-requests/src/lib/requests-result';
import { catchError, distinctUntilChanged, EMPTY, filter, first, map, Observable, of, shareReplay, switchMap, throwError, timeout } from 'rxjs';
import { AuthenticationService, Profile } from 'simple-blog/core';

@Injectable()
export class ProfileService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
    private readonly _authenticationService: AuthenticationService
  ) {

    this._currentProfile = this._authenticationService.isAuthenticatedAsync().pipe(
      distinctUntilChanged(),
      switchMap(isAuthenticated => isAuthenticated ? this._http.get<Profile>(this._urls.getCurrent(), { headers: { 'Authorization': '' } }) : of(null)),
      switchMap(profile => profile ? this._profileStore.pipe(selectEntity(profile.id), map(p => p ? p : profile)) : of(null)),
      shareReplay(),
    );

  }

  public getCurrentAsync(): Observable<Profile | null> {
    return this._currentProfile;
  }

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

  private readonly _currentProfile: Observable<Profile | null>;

  private readonly _profileStore = createStore(
    { name: 'profile-store' },
    withEntities<Profile>(),
  );

  private readonly _urls = {
    getCurrent: () => `${this._baseUrl}api/profiles/current`,
    getById: (id: string) => `${this._baseUrl}api/profiles/${id}`,
  }
}
