import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { BehaviorSubject, finalize } from 'rxjs';
import { AccountService } from 'simple-blog/core';

@Component({
  selector: 'account-settings',
  templateUrl: './settings.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsComponent {

  readonly form = new FormGroup({
    currentPassword: new FormControl(''),
    newPassword: new FormControl(''),
  });

  isUpdated = new BehaviorSubject<boolean>(false);

  constructor(
    private readonly _accountService: AccountService,
  ) { }

  onSubmit(): void {
    this.form.disable();
    let data = this.form.value;

    this._accountService.updatePassword({ currentPassword: data.currentPassword!, newPassword: data.newPassword! }).pipe(
      finalize(() => this.form.enable()),
    ).subscribe(
      next => { this.isUpdated.next(true); },
    );
  }

}
