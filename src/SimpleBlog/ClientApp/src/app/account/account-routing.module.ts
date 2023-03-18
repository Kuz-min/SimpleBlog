import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { isAuthenticatedGuard } from 'simple-blog/core';
import { SettingsComponent } from './settings/settings.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';

const routes: Routes = [
  { path: 'account/settings', component: SettingsComponent, canActivate: [isAuthenticatedGuard] },
  { path: 'account/sign-in', component: SignInComponent },
  { path: 'account/sign-up', component: SignUpComponent },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
})
export class AccountRoutingModule { }

