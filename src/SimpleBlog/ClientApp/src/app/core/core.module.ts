import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { AddTimeoutInterceptor, AddTokenInterceptor } from './interceptors';
import { AccountRoleService, AccountService, AuthenticationService, AuthorizationService, PostService, PostTagService, ProfileService } from './services';

@NgModule({
  providers: [
    //interceptors
    { provide: HTTP_INTERCEPTORS, useClass: AddTimeoutInterceptor, multi: true },
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
