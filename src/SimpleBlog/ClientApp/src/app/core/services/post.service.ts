import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { first, mergeMap, Observable, shareReplay, tap, throwError, timeout } from 'rxjs';
import { Post } from 'simple-blog/core';
import { Cache } from '../utilities';

@Injectable()
export class PostService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: number): Observable<Post | null> {

    if (!this._cache.contains(id)) {
      this._http.get<Post>(this._urls.getById(id)).pipe(
        first(),
        timeout(3000),
      ).subscribe(
        next => this._cache.set(next.id, next),
        error => console.log(error),
      );
    }

    return this._cache.getOrCreate(id);
  }

  public searchAsync(tagIds?: number[], offset?: number, count?: number): Observable<Post[] | null> {
    let params = new HttpParams();

    if (tagIds)
      params = params.append('tag_ids', tagIds.join(','));

    if (offset)
      params = params.append('offset', offset);

    if (count)
      params = params.append('count', count);

    return this._http.get<Post[]>(this._urls.search(), { params: params }).pipe(
      first(),
      timeout(3000),
      tap(posts => posts.forEach(post => this._cache.set(post.id, post))),
      shareReplay(),
    );
  }

  public createAsync(data: { title: string, content: string, tagIds?: number[] }): Observable<Post | null> {
    return this._http.post<Post>(this._urls.create(), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      mergeMap(post => this._cache.set(post.id, post)),
      shareReplay(),
    );
  }

  public updateAsync(id: number, data: { title: string, content: string, tagIds?: number[] }): Observable<Post | null> {
    return this._http.put<Post>(this._urls.update(id), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      mergeMap(post => this._cache.set(post.id, post)),
      shareReplay(),
    );
  }

  public deleteAsync(id: number): Observable<any> {
    return this._http.delete<any>(this._urls.delete(id), { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      tap(_ => this._cache.remove(id)),
      shareReplay(),
    );
  }

  private readonly _cache = new Cache<Post>();

  private readonly _urls = {
    getById: (id: number) => `${this._baseUrl}api/posts/${id}`,
    search: () => `${this._baseUrl}api/posts/search`,
    create: () => `${this._baseUrl}api/posts`,
    update: (id: number) => `${this._baseUrl}api/posts/${id}`,
    delete: (id: number) => `${this._baseUrl}api/posts/${id}`,
  };

}
