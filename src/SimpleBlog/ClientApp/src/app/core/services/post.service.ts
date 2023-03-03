import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { createStore } from '@ngneat/elf';
import { deleteEntitiesByPredicate, selectEntity, upsertEntities, withEntities } from '@ngneat/elf-entities';
import { getRequestResult, joinRequestResult, trackRequestResult } from '@ngneat/elf-requests';
import { ErrorRequestResult } from '@ngneat/elf-requests/src/lib/requests-result';
import { catchError, EMPTY, filter, first, map, Observable, of, shareReplay, switchMap, tap, throwError, timeout } from 'rxjs';
import { Post } from 'simple-blog/core';

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
      filter(request => !(request.isLoading)),
      switchMap(() => this._http.get<Post>(this._urls.getById(id)).pipe(
        first(),
        timeout(3000),
        trackRequestResult(key, { staleTime: 30_000 }),
      )),
      catchError(() => EMPTY),
    ).subscribe(
      next => this._postStore.update(upsertEntities(next)),
    );

    return this._postStore.pipe(
      selectEntity(id),
      joinRequestResult(key),
      filter(request => request.status != 'idle' && request.status != 'loading'),
      switchMap(request => request.isSuccess ? of(request.data as Post) : throwError((request as ErrorRequestResult)?.error)),
    );
  }


  public searchAsync(tagIds?: number[], offset?: number, count?: number): Observable<Post[]> {
    let params = new HttpParams();

    if (tagIds)
      params = params.append('tag_ids', tagIds.sort((a, b) => a - b).join(','));

    if (offset)
      params = params.append('offset', offset);

    if (count)
      params = params.append('count', count);

    return this._http.get<Post[]>(this._urls.search(), { params: params }).pipe(
      first(),
      timeout(3000),
      tap(posts => of(posts).pipe(
        map(posts => posts.map(post => of(post).pipe(
          trackRequestResult(['post', post.id], { staleTime: 30_000 }),
          tap(p => this._postStore.update(upsertEntities(p))),
        ).subscribe())),
      ).subscribe()),
      shareReplay(),
    );
  }

  public createAsync(data: { title: string, content: string, image?: File, tagIds?: number[] }): Observable<Post> {
    const formData = new FormData();
    formData.append("title", data.title);
    formData.append("content", data.content);
    if (data.image)
      formData.append("image", data.image);
    if (data.tagIds)
      data.tagIds.forEach(value => formData.append("tagIds[]", value.toString()));

    return this._http.post<Post>(this._urls.create(), formData, { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      tap(post => this._postStore.update(upsertEntities(post))),
      switchMap(post => this._postStore.pipe(
        selectEntity(post.id),
        filter(p => Boolean(p)),
        map(p => p as Post),
      )),
      shareReplay(),
    );
  }

  public updateAsync(id: number, data: { title: string, content: string, image?: File, tagIds?: number[] }): Observable<Post> {
    const formData = new FormData();
    formData.append("title", data.title);
    formData.append("content", data.content);
    if (data.image)
      formData.append("image", data.image);
    if (data.tagIds)
      data.tagIds.forEach(value => formData.append("tagIds[]", value.toString()));

    return this._http.put<Post>(this._urls.update(id), formData, { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      tap(post => this._postStore.update(upsertEntities(post))),
      switchMap(() => this._postStore.pipe(
        selectEntity(id),
        filter(post => Boolean(post)),
        map(post => post as Post),
      )),
      shareReplay(),
    );
  }

  public deleteAsync(id: number): Observable<any> {
    return this._http.delete<any>(this._urls.delete(id), { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      tap(() => this._postStore.update(deleteEntitiesByPredicate(post => post.id == id))),
      shareReplay(),
    );
  }

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
