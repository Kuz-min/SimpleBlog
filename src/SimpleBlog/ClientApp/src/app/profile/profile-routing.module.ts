import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { ProfileComponent } from './profile/profile.component';

const routes: Routes = [
  { path: 'profiles/:id', component: ProfileComponent },
  { path: 'profiles/:id/edit', component: ProfileEditorComponent },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
  ],
  exports: [
    RouterModule,
  ],
})
export class ProfileRoutingModule { }
