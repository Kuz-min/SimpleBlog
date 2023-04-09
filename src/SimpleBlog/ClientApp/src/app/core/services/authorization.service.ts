import { Injectable } from '@angular/core';
import { combineLatest, map, Observable } from 'rxjs';
import { AccountRoleService, AuthenticationService, PermissionsConstants, Post } from 'simple-blog/core';

@Injectable()
export class AuthorizationService {

  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _accountRoleService: AccountRoleService,
  ) { }

  public isAuthorizedToEditPost(post: Observable<Post | null>): Observable<boolean> {

    const isOwner = combineLatest([this._authService.getIdAsync(), post]).pipe(
      map(([accountId, post]) => accountId && post && post.ownerId == accountId),
    );

    const isInRole = combineLatest([this._authService.getRolesAsync(), this._accountRoleService.getAllAsync()]).pipe(
      map(([accountRoleNames, roles]) => roles.filter(role => accountRoleNames?.includes(role.name))),
      map(accountRoles => accountRoles.map(role => role.permissions.includes(PermissionsConstants.PostFullAccess)).includes(true)),
    );

    return combineLatest([post, isOwner, isInRole]).pipe(
      map(([post, isOwner, inRole]) => post != null && (isOwner || inRole)),
    );

  }

  public isAuthorizedToEditPostTags(): Observable<boolean> {
    return combineLatest([this._authService.getRolesAsync(), this._accountRoleService.getAllAsync()]).pipe(
      map(([accountRoles, roles]) => roles.filter(role => accountRoles?.includes(role.name))),
      map(roles => roles.map(role => role.permissions.includes(PermissionsConstants.PostTagFullAccess)).includes(true)),
    );
  }

}
