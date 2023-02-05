import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, EMPTY, finalize, switchMap, throwError } from 'rxjs';
import { AccountService, AuthenticationService } from 'simple-blog/core';

@Component({
  selector: 'account-sign-up',
  templateUrl: './sign-up.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignUpComponent {

  readonly form = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(/^[a-z]+[a-z0-9|\-|_]*[a-z0-9]$/i)]),
    email: new FormControl('', [Validators.required, Validators.pattern(/^[^@\s]+@[^@\s]+$/i)]),
    password: new FormControl('', [Validators.required]),
  });

  constructor(
    private readonly _router: Router,
    private readonly _accountService: AccountService,
    private readonly _authService: AuthenticationService
  ) { }

  onSubmit(): void {
    this.form.disable();
    let data = this.form.value;

    this._accountService.createAsync({ username: data.username!, email: data.email!, password: data.password! }).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status == 400) {
          //this.form.controls.username.setErrors({ 'pattern': true });
          //this.form.controls.email.setErrors({ 'pattern': true });
          return EMPTY;
        }
        return throwError(error);
      }),
      switchMap(() => this._authService.signInAsync({ username: data.username!, password: data.password! })),
      finalize(() => this.form.enable()),
    ).subscribe(
      next => this._router.navigate(['/']),
      error => { },
    );
  }

}
