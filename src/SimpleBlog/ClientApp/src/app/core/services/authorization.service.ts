import { Injectable } from '@angular/core';
import { combineLatest, map, Observable } from 'rxjs';
import { AccountRoleService, AuthenticationService, Post } from 'simple-blog/core';

@Injectable()
export class AuthorizationService {

  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _accountRoleService: AccountRoleService,
  ) { }

  public isAuthorizedToEditPost(post: Observable<Post | null>): Observable<boolean> {
    return combineLatest(this._authService.getIdAsync(), post).pipe(
      map(([id, post]) => id && post && post.ownerId == id ? true : false),
    );
  }

  public isAuthorizedToEditPostTags(): Observable<boolean> {
    return combineLatest(this._authService.getRolesAsync(), this._accountRoleService.getAllAsync()).pipe(
      map(([accountRoles, roles]) => roles.filter(role => accountRoles?.includes(role.name))),
      map(roles => roles.map(role => role.permissions.includes('PostTagFullAccess')).includes(true)),
    );
  }

}
