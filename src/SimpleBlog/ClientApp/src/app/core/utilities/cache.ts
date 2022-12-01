import { BehaviorSubject, Observable } from "rxjs";

export class Cache<T> {

  public contains(id: number): boolean {
    return Boolean(this._items[id]);
  }

  public set(id: number, item: T): Observable<T | null> {
    if (this.contains(id))
      this._items[id].next(item);
    else
      this._items[id] = new BehaviorSubject<T | null>(item);

    return this._items[id].asObservable();
  }

  public getOrCreate(id: number): Observable<T | null> {
    if (!this.contains(id))
      this._items[id] = new BehaviorSubject<T | null>(null);

    return (this._items[id]);
  }

  public remove(id: number): void {
    if (this.contains(id))
      this._items.splice(id);
  }

  private readonly _items: BehaviorSubject<T | null>[] = [];
}
