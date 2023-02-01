import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService, AuthorizationService } from 'simple-blog/core';

@Component({
  selector: 'shared-main-side-menu',
  templateUrl: './main-side-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MainSideMenuComponent implements OnInit {

  isAuthenticated: (Observable<boolean> | null) = null;

  isAuthorizedToEditPostTags: (Observable<boolean> | null) = null;

  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _authorizationService: AuthorizationService,
  ) { }

  ngOnInit(): void {
    this.isAuthenticated = this._authService.isAuthenticatedAsync();

    this.isAuthorizedToEditPostTags = this._authorizationService.isAuthorizedToEditPostTags();
  }

}
