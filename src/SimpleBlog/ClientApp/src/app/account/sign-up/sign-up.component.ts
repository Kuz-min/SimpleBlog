import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, EMPTY, finalize, switchMap, throwError } from 'rxjs';
import { AccountService, AuthenticationService, ValidationConstants } from 'simple-blog/core';

@Component({
  selector: 'account-sign-up',
  templateUrl: './sign-up.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignUpComponent {

  readonly form = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(ValidationConstants.USERNAME_REG_EX)]),
    email: new FormControl('', [Validators.required, Validators.pattern(ValidationConstants.EMAIL_REG_EX)]),
    password: new FormControl('', [Validators.required]),
  });

  readonly serverResponse = new BehaviorSubject<number>(0);

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
        if (error instanceof HttpErrorResponse) {
          this.serverResponse.next(error.status);
          return EMPTY;
        }
        return throwError(error);
      }),
      switchMap(() => this._authService.signInAsync({ username: data.username!, password: data.password! })),
      finalize(() => { this.form.enable(); this.form.markAsUntouched(); }),
    ).subscribe(
      next => this._router.navigate(['/']),
    );
  }

}
