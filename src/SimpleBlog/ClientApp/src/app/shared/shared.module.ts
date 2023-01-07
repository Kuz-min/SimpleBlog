import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { DefaultLayoutComponent } from './default-layout/default-layout.component';
import { HeaderComponent } from './header/header.component';
import { MainSideMenuComponent } from './main-side-menu/main-side-menu.component';
import { ProfileSideMenuComponent } from './profile-side-menu/profile-side-menu.component';

@NgModule({
  declarations: [
    DefaultLayoutComponent,
    HeaderComponent,
    MainSideMenuComponent,
    ProfileSideMenuComponent,
  ],
  imports: [
    BrowserAnimationsModule,
    CommonModule,
    RouterModule,

    MatSidenavModule,
    MatIconModule,
  ],
  exports: [
    DefaultLayoutComponent,
  ],
})
export class SharedModule { }
