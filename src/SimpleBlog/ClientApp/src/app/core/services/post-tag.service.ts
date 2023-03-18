import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { deleteEntitiesByPredicate, selectAllEntities, selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { ErrorRequestResult } from '@ngneat/elf-requests/src/lib/requests-result';
import { catchError, EMPTY, filter, first, map, Observable, of, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { PostTag } from 'simple-blog/core';

@Injectable()
export class PostTagService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: number): Observable<PostTag> {
    const key = ['post-tag-all'];

    this.getAllAsync().subscribe();

    return this._postTagStore.pipe(
      selectEntity(id),
      joinRequestResult(key),
      filter(request => request.status != 'idle' && request.status != 'loading'),
      switchMap(request => request.isSuccess ? of(request.data as PostTag) : throwError((request as ErrorRequestResult).error)),
    );
  }

  public getAllAsync(): Observable<PostTag[]> {
    const key = ['post-tag-all'];

    getRequestResult(key, { initialStatus: 'idle' }).pipe(
      first(),
      filter(request => !(request.isLoading)),
      switchMap(() => this._http.get<PostTag[]>(this._urls.getAll()).pipe(
        first(),
        catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? of([]) : throwError(error)),
        trackRequestResult(key, { staleTime: 30_000 }),
      )),
      catchError(() => EMPTY),
    ).subscribe(
      next => this._postTagStore.update(upsertEntities(next)),
    );

    return this._postTagStore.pipe(
      selectAllEntities(),
      joinRequestResult(key),
      filter(request => request.status != 'idle' && request.status != 'loading'),
      switchMap(request => request.isSuccess ? of(request.data as PostTag[]) : throwError((request as ErrorRequestResult).error)),
    );
  }

  public createAsync(data: { title: string }): Observable<PostTag> {
    return this._http.post<PostTag>(this._urls.create(), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(postTag => this._postTagStore.update(upsertEntities(postTag))),
      switchMap(postTag => this._postTagStore.pipe(
        selectEntity(postTag.id),
        filter(p => Boolean(p)),
        map(p => p as PostTag),
      )),
      shareReplay(),
    );
  }

  public updateAsync(id: number, data: { title: string }): Observable<PostTag> {
    return this._http.put<PostTag>(this._urls.update(id), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(postTag => this._postTagStore.update(upsertEntities(postTag))),
      switchMap(() => this._postTagStore.pipe(
        selectEntity(id),
        filter(postTag => Boolean(postTag)),
        map(postTag => postTag as PostTag),
      )),
      shareReplay(),
    );
  }

  public deleteAsync(id: number): Observable<any> {
    return this._http.delete<any>(this._urls.delete(id), { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(() => this._postTagStore.update(deleteEntitiesByPredicate(postTag => postTag.id == id))),
      shareReplay(),
    );
  }

  private readonly _postTagStore = createStore(
    { name: 'post-tag-store' },
    withEntities<PostTag>(),
  );

  private readonly _urls = {
    //getById: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
    getAll: () => `${this._baseUrl}api/post-tags`,
    create: () => `${this._baseUrl}api/post-tags`,
    update: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
    delete: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
  };

}
