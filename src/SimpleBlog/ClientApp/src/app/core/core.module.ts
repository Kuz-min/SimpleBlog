import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { IsAuthenticatedGuard } from './guards';
import { AddTokenInterceptor } from './interceptors';
import { AccountService, AuthenticationService, AuthorizationService, PostService, PostTagService, ProfileService } from './services';

@NgModule({
  providers: [
    IsAuthenticatedGuard,
    { provide: HTTP_INTERCEPTORS, useClass: AddTokenInterceptor, multi: true },
    AccountService,
    AuthenticationService,
    AuthorizationService,
    PostService,
    PostTagService,
    ProfileService,
  ],
})
export class CoreModule { }
