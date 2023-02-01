import { ChangeDetectionStrategy, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService, AuthorizationService } from 'simple-blog/core';

@Component({
  selector: 'shared-header',
  templateUrl: './header.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent implements OnInit {

  @Output() readonly menuToggle = new EventEmitter<boolean>();
  @Output() readonly profileToggle = new EventEmitter<boolean>();

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
