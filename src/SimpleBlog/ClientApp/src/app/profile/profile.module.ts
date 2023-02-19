import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { ProfilePreviewComponent } from './profile-preview/profile-preview.component';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileComponent } from './profile/profile.component';

@NgModule({
  declarations: [
    ProfileComponent,
    ProfileEditorComponent,
    ProfilePreviewComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    ProfileRoutingModule,
  ],
  exports: [
    ProfilePreviewComponent,
  ],
})
export class ProfileModule { }
