import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService, Profile, ProfileService } from 'simple-blog/core';

@Component({
  selector: 'shared-profile-side-menu',
  templateUrl: './profile-side-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileSideMenuComponent implements OnInit {

  isAuthenticated: (Observable<boolean> | null) = null;
  currentProfile: (Observable<Profile | null> | null) = null;

  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _profileService: ProfileService,
  ) { }

  ngOnInit(): void {
    this.isAuthenticated = this._authService.isAuthenticatedAsync();
    this.currentProfile = this._profileService.getCurrentAsync();
  }

  signOut(): void {
    this._authService.signOut();
  }

}
