import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { BrowserModule } from '@angular/platform-browser';
import { OAuthModule, OAuthStorage } from 'angular-oauth2-oidc';
import { AccountModule } from './account/account.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { HomeComponent } from './home/home.component';
import { PostModule } from './post/post.module';
import { ProfileModule } from './profile/profile.module';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
  ],
  imports: [
    CoreModule,
    BrowserModule, //.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    HttpClientModule,

    MatListModule,

    OAuthModule.forRoot(),

    AccountModule,
    PostModule,
    ProfileModule,
    SharedModule,

    AppRoutingModule,//Shuld be last, else rout 404 ** handle all paths
  ],
  providers: [
    { provide: OAuthStorage, useFactory: () => localStorage },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
