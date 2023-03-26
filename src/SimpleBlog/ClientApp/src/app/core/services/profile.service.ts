import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { catchError, EMPTY, filter, first, map, Observable, of, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { Profile, ServerApiConstants } from 'simple-blog/core';

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
      filter(request => !request.isLoading),
      switchMap(() => this._http.get<Profile>(this._urls.getById(id)).pipe(
        first(),
        tap(profile => this._profileStore.update(upsertEntities(profile))),
        trackRequestResult(key, { staleTime: ServerApiConstants.DefaultCacheTimeout }),
      )),
      catchError(() => EMPTY),
    ).subscribe();

    return this._profileStore.pipe(
      selectEntity(id),
      joinRequestResult(key),
      filter(request => request.isSuccess || request.isError),
      switchMap(request => request.isSuccess ? of(request.data!) : request.isError ? throwError(() => request.error) : EMPTY),
    );
  }

  public updateImageAsync(id: string, data: File): Observable<Profile> {
    const formData = new FormData();
    formData.append("file", data);

    return this._http.put<Profile>(this._urls.updateImage(id), formData, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(profile => this._profileStore.update(upsertEntities(profile))),
      switchMap(() => this._profileStore.pipe(
        selectEntity(id),
        map(profile => profile as Profile),
      )),
      shareReplay(),
    );
  }

  private readonly _profileStore = createStore(
    { name: 'profile-store' },
    withEntities<Profile>(),
  );

  private readonly _urls = {
    getById: (id: string) => `${this._baseUrl}api/profiles/${id}`,
    updateImage: (id: string) => `${this._baseUrl}api/profiles/${id}/image`,
  }
}
