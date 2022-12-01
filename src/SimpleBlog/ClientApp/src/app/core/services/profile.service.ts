import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { first, Observable, tap, timeout } from 'rxjs';
import { Profile } from 'simple-blog/core';
import { Cache } from '../utilities';

@Injectable()
export class ProfileService {

  constructor(
    @Inject('BASE_URL') private readonly _baseUrl: string,
    private readonly _http: HttpClient,
  ) { }

  public getByIdAsync(id: number): Observable<Profile | null> {

    if (!this._cache.contains(id)) {
      this._http.get<Profile>(this._urls.getById(id)).pipe(
        first(),
        timeout(3000),
      ).subscribe(
        next => this._cache.set(id, next),
      );
    }

    return this._cache.getOrCreate(id);
  }

  private readonly _cache = new Cache<Profile>();

  private readonly _urls = {
    getById: (id: number) => `${this._baseUrl}api/profiles/${id}`,
  }
}
