import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { IsAuthenticatedGuard } from './guards';
import { AddTokenInterceptor } from './interceptors';
import { AccountService, AuthenticationService, PostService, PostTagService, ProfileService, ScrollService } from './services';

@NgModule({
  providers: [
    IsAuthenticatedGuard,
    { provide: HTTP_INTERCEPTORS, useClass: AddTokenInterceptor, multi: true },
    AccountService,
    AuthenticationService,
    PostService,
    PostTagService,
    ProfileService,
    ScrollService,
  ],
})
export class CoreModule { }
