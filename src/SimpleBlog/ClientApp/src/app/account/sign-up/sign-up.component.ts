import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { concatMap, finalize } from 'rxjs';
import { AccountService, AuthenticationService } from 'simple-blog/core';

@Component({
  selector: 'account-sign-up',
  templateUrl: './sign-up.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignUpComponent {

  readonly form = new FormGroup({
    username: new FormControl(''),
    email: new FormControl(''),
    password: new FormControl(''),
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
      concatMap(() => this._authService.signInAsync({ username: data.username!, password: data.password! })),
      finalize(() => this.form.enable()),
    ).subscribe(
      next => this._router.navigate(['/']),
      error => console.error(error),
    );
  }

}
