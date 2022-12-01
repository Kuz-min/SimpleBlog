import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileComponent } from './profile/profile.component';
import { ProfilePreviewComponent } from './profile-preview/profile-preview.component';

@NgModule({
  declarations: [
    ProfileComponent,
    ProfilePreviewComponent,
  ],
  imports: [
    CommonModule,

    ProfileRoutingModule,
  ],
  exports: [
    ProfilePreviewComponent,
  ],
})
export class ProfileModule { }
