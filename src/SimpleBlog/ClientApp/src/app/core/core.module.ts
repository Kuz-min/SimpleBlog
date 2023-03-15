import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { IsAuthenticatedGuard, IsAuthorizedToEditPostGuard, IsAuthorizedToEditPostTagsGuard } from './guards';
import { AddTokenInterceptor } from './interceptors';
import { AccountRoleService, AccountService, AuthenticationService, AuthorizationService, PostService, PostTagService, ProfileService } from './services';

@NgModule({
  providers: [
    //guards
    IsAuthenticatedGuard,
    IsAuthorizedToEditPostGuard,
    IsAuthorizedToEditPostTagsGuard,

    //interceptors
    { provide: HTTP_INTERCEPTORS, useClass: AddTokenInterceptor, multi: true },

    //services
    AccountService,
    AccountRoleService,
    AuthenticationService,
    AuthorizationService,
    PostService,
    PostTagService,
    ProfileService,
  ],
})
export class CoreModule { }
