import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { deleteEntitiesByPredicate, selectAllEntities, selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { catchError, EMPTY, filter, first, Observable, of, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { filterNotNullAndNotUndefined, PostTag, ServerApiConstants } from 'simple-blog/core';

@Injectable()
export class PostTagService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: number): Observable<PostTag> {
    const key = ['post-tag-all'];

    this.getAllAsync().pipe(
      first(),
      catchError(() => EMPTY),
    ).subscribe();

    return this._postTagStore.pipe(
      selectEntity(id),
      joinRequestResult(key),
      filter(request => request.isSuccess || request.isError),
      switchMap(request => request.isSuccess ? of(request.data!) : request.isError ? throwError(() => request.error) : EMPTY),
    );
  }

  public getAllAsync(): Observable<PostTag[]> {
    const key = ['post-tag-all'];

    getRequestResult(key, { initialStatus: 'idle' }).pipe(
      first(),
      filter(request => !(request.isLoading)),
      switchMap(() => this._http.get<PostTag[]>(this._urls.getAll()).pipe(
        first(),
        tap(tags => this._postTagStore.update(upsertEntities(tags))),
        trackRequestResult(key, { staleTime: ServerApiConstants.DefaultCacheTimeout }),
      )),
      catchError(() => EMPTY),
    ).subscribe();

    return this._postTagStore.pipe(
      selectAllEntities(),
      joinRequestResult(key),
      filter(request => request.isSuccess || request.isError),
      switchMap(request => request.isSuccess ? of(request.data) : request.isError ? throwError(() => request.error) : EMPTY),
    );
  }

  public createAsync(data: { title: string }): Observable<PostTag> {
    return this._http.post<PostTag>(this._urls.create(), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(tag => this._postTagStore.update(upsertEntities(tag))),
      switchMap(tag => this._postTagStore.pipe(
        selectEntity(tag.id),
        filterNotNullAndNotUndefined(),
      )),
      shareReplay(),
    );
  }

  public updateAsync(id: number, data: { title: string }): Observable<PostTag> {
    return this._http.put<PostTag>(this._urls.update(id), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(tag => this._postTagStore.update(upsertEntities(tag))),
      switchMap(() => this._postTagStore.pipe(
        selectEntity(id),
        filterNotNullAndNotUndefined(),
      )),
      shareReplay(),
    );
  }

  public deleteAsync(id: number): Observable<any> {
    return this._http.delete<any>(this._urls.delete(id), { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(() => this._postTagStore.update(deleteEntitiesByPredicate(tag => tag.id == id))),
      shareReplay(),
    );
  }

  private readonly _postTagStore = createStore(
    { name: 'post-tag-store' },
    withEntities<PostTag>(),
  );

  private readonly _urls = {
    getAll: () => `${this._baseUrl}api/post-tags`,
    create: () => `${this._baseUrl}api/post-tags`,
    update: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
    delete: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
  };

}
