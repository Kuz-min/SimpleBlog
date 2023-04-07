import { filter, Observable, OperatorFunction, pipe, UnaryFunction } from "rxjs";

export function filterNotNullAndNotUndefined<T>(): UnaryFunction<Observable<T | null | undefined>, Observable<T>> {
  return pipe(
    filter(o => o !== undefined && o !== null) as OperatorFunction<T | null | undefined, T>
  );
}
