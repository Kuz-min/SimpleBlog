import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, EMPTY, finalize, throwError } from 'rxjs';
import { AuthenticationService } from 'simple-blog/core';

@Component({
  selector: 'account-sign-in',
  templateUrl: './sign-in.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignInComponent {

  readonly form = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(/^[a-z]+[a-z0-9|\-|_]*[a-z0-9]$/i)]),
    password: new FormControl('', [Validators.required]),
  });

  constructor(
    private readonly _router: Router,
    private readonly _authService: AuthenticationService
  ) { }

  onSubmit(): void {
    this.form.disable();
    let data = this.form.value;

    this._authService.signInAsync({ username: data.username!, password: data.password! }).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status == 400) {
          //this.form.controls.password.setErrors({ 'server': true });
          return EMPTY;
        }
        return throwError(error);
      }),
      finalize(() => this.form.enable()),
    ).subscribe(
      next => this._router.navigate(['/']),
      error => { },
    );
  }

}
