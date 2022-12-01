import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { first, mergeMap, Observable, shareReplay, tap, timeout } from 'rxjs';
import { PostTag } from 'simple-blog/core';
import { Cache } from '../utilities';

@Injectable()
export class PostTagService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: number): Observable<PostTag | null> {

    if (!this._cache.contains(id)) {
      this._http.get<PostTag>(this._urls.getById(id)).pipe(
        first(),
        timeout(3000),
      ).subscribe(
        next => this._cache.set(next.id, next),
      );
    }

    return this._cache.getOrCreate(id);
  }

  public getAllAsync(): Observable<PostTag[] | null> {
    return this._http.get<PostTag[]>(this._urls.getAll()).pipe(
      first(),
      timeout(3000),
      tap(tags => tags.forEach(tag => this._cache.set(tag.id, tag))),
      shareReplay(),
    );
  }

  public createAsync(data: { title: string }): Observable<PostTag | null> {
    return this._http.post<PostTag>(this._urls.create(), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      mergeMap(tag => this._cache.set(tag.id, tag)),
      shareReplay(),
    );
  }

  public updateAsync(id: number, data: { title: string }): Observable<PostTag | null> {
    return this._http.put<PostTag>(this._urls.update(id), data, { headers: { 'Authorization': '' } }).pipe(
      first(),
      timeout(3000),
      mergeMap(tag => this._cache.set(tag.id, tag)),
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

  private readonly _cache = new Cache<PostTag>();

  private readonly _urls = {
    getById: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
    getAll: () => `${this._baseUrl}api/post-tags`,
    create: () => `${this._baseUrl}api/post-tags`,
    update: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
    delete: (id: number) => `${this._baseUrl}api/post-tags/${id}`,
  };

}
