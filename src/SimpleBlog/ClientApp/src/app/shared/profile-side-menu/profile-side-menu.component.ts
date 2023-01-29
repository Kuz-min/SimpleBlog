import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService } from 'simple-blog/core';

@Component({
  selector: 'shared-profile-side-menu',
  templateUrl: './profile-side-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileSideMenuComponent implements OnInit {

  isAuthenticated: (Observable<boolean> | null) = null;
  id: (Observable<string | null> | null) = null;
  name: (Observable<string | null> | null) = null;

  constructor(
    private readonly _authService: AuthenticationService,
  ) { }

  ngOnInit(): void {
    this.isAuthenticated = this._authService.isAuthenticatedAsync();

    this.id = this._authService.getIdAsync();
    this.name = this._authService.getNameAsync();
  }

  signOut(): void {
    this._authService.signOut();
  }

}
