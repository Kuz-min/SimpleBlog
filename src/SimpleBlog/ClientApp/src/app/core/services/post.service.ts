import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { deleteAllEntities, deleteEntitiesByPredicate, getEntitiesIds, selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { deleteRequestResult, getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { catchError, EMPTY, filter, first, from, map, Observable, of, shareReplay, switchMap, tap, throwError, toArray } from 'rxjs';
import { List, Post, PostFormModel, ServerApiConstants } from 'simple-blog/core';

@Injectable()
export class PostService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: number): Observable<Post> {
    const key = ['post', id];

    getRequestResult(key, { initialStatus: 'idle' }).pipe(
      first(),
      filter(request => !request.isLoading),
      switchMap(() => this._http.get<Post>(this._urls.getById(id)).pipe(
        first(),
        tap(post => this._postStore.update(upsertEntities(post))),
        trackRequestResult(key, { staleTime: ServerApiConstants.DefaultCacheTimeout }),
      )),
      catchError(() => EMPTY),
    ).subscribe();

    return this._postStore.pipe(
      selectEntity(id),
      joinRequestResult(key),
      filter(request => request.isSuccess || request.isError),
      switchMap(request => request.isSuccess ? of(request.data!) : request.isError ? throwError(() => request.error) : EMPTY),
    );
  }

  public searchAsync(tagIds?: number[], offset?: number, count?: number): Observable<List<number>> {
    const sortedTagIds = tagIds?.slice().sort((a, b) => a - b);

    let params = new HttpParams();
    if (sortedTagIds)
      params = params.append('tag_ids', sortedTagIds.join(','));
    if (offset)
      params = params.append('offset', offset);
    if (count)
      params = params.append('count', count);

    const key = `post-search-${sortedTagIds?.join(',') ?? ''}-${offset ?? ''}-${count ?? ''}`;

    getRequestResult([key], { initialStatus: 'idle' }).pipe(
      first(),
      filter(request => !request.isLoading),
      switchMap(() => this._http.get<List<Post>>(this._urls.search(), { params: params }).pipe(
        first(),
        switchMap(list => from(list.items).pipe(
          //saves each post in cache separately
          tap(post => this._postStore.update(upsertEntities(post))),
          //prevents later requests for saved in cache posts
          //tap + of + subscribe avoids trackRequestResult complete effect because simple way to set RequestResult not exist
          tap(post => of(post).pipe(trackRequestResult(['post', post.id], { staleTime: ServerApiConstants.DefaultCacheTimeout })).subscribe()),
          map(post => post.id),
          toArray(),
          map(ids => ({ offset: list.offset, length: list.length, items: ids }) as List<number>),
        )),
        //saves post ids in cache
        tap(list => this._postSearchStore.update(upsertEntities({ id: key, list: list }))),
        //saves request status
        //cancels request if cache timeout not expired
        trackRequestResult([key], { staleTime: ServerApiConstants.DefaultCacheTimeout }),
      )),
      catchError(() => EMPTY),
    ).subscribe();

    return this._postSearchStore.pipe(
      selectEntity(key),
      joinRequestResult([key]),
      filter(request => request.isSuccess || request.isError),
      switchMap(request => request.isSuccess ? of(request.data!.list) : request.isError ? throwError(() => request.error) : EMPTY),
    );
  }

  public createAsync(data: PostFormModel): Observable<Post> {
    const formData = new FormData();
    formData.append("title", data.title);
    formData.append("content", data.content);
    if (data.image)
      formData.append("image", data.image);
    if (data.tagIds)
      data.tagIds.forEach(value => formData.append("tagIds[]", value.toString()));

    return this._http.post<Post>(this._urls.create(), formData, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(post => this._postStore.update(upsertEntities(post))),
      tap(() => this.clearSearchCache()),
      switchMap(post => this._postStore.pipe(
        selectEntity(post.id),
        map(p => p as Post),
      )),
      shareReplay(),
    );
  }

  public updateAsync(id: number, data: PostFormModel): Observable<Post> {
    const formData = new FormData();
    formData.append("title", data.title);
    formData.append("content", data.content);
    if (data.image)
      formData.append("image", data.image);
    if (data.tagIds)
      data.tagIds.forEach(value => formData.append("tagIds[]", value.toString()));

    return this._http.put<Post>(this._urls.update(id), formData, { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(post => this._postStore.update(upsertEntities(post))),
      tap(() => this.clearSearchCache()),
      switchMap(() => this._postStore.pipe(
        selectEntity(id),
        map(post => post as Post),
      )),
      shareReplay(),
    );
  }

  public deleteAsync(id: number): Observable<any> {
    return this._http.delete<any>(this._urls.delete(id), { headers: { 'Authorization': '' } }).pipe(
      first(),
      tap(() => this._postStore.update(deleteEntitiesByPredicate(post => post.id == id))),
      tap(() => this.clearSearchCache()),
      shareReplay(),
    );
  }

  private clearSearchCache() {
    this._postSearchStore.query(getEntitiesIds()).forEach(id => deleteRequestResult([id]));
    this._postSearchStore.update(deleteAllEntities());
    console.log('search cache cleared');
  }

  private readonly _postSearchStore = createStore(
    { name: 'post-search-store' },
    withEntities<{ id: string, list: List<number> }>(),
  );

  private readonly _postStore = createStore(
    { name: 'post-store' },
    withEntities<Post>(),
  );

  private readonly _urls = {
    getById: (id: number) => `${this._baseUrl}api/posts/${id}`,
    search: () => `${this._baseUrl}api/posts/search`,
    create: () => `${this._baseUrl}api/posts`,
    update: (id: number) => `${this._baseUrl}api/posts/${id}`,
    delete: (id: number) => `${this._baseUrl}api/posts/${id}`,
  };

}
