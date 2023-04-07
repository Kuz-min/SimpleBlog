import { HttpErrorResponse } from "@angular/common/http";
import { catchError, Observable, OperatorFunction, pipe, throwError, UnaryFunction } from "rxjs";

export function catchNotFound<T, TNew>(newSource: Observable<TNew>): UnaryFunction<Observable<T>, Observable<TNew>> {
  return pipe(
    catchError(error => error instanceof HttpErrorResponse && error.status == 404 ? newSource : throwError(() => error)) as OperatorFunction<T, TNew>
  );
}
