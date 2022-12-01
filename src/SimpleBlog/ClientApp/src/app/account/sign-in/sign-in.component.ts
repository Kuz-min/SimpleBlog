import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
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
    username: new FormControl(''),
    password: new FormControl(''),
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
        if ((error as HttpErrorResponse)?.status == 400) {
          //this.signInForm.controls.username.setErrors({ 'incorrect': true });
          return EMPTY;
        }

        return throwError(error);
      }),
      finalize(() => this.form.enable()),
    ).subscribe(
      next => this._router.navigate(['/']),
      error => console.error(error),
    );
  }

}
