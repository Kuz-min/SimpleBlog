import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ProfileModule } from 'simple-blog/profile/profile.module';
import { PostChangedAlertComponent } from './post-changed-alert/post-changed-alert.component';
import { PostCreatorComponent } from './post-creator/post-creator.component';
import { PostEditorComponent } from './post-editor/post-editor.component';
import { PostFormComponent } from './post-form/post-form.component';
import { PostPreviewComponent } from './post-preview/post-preview.component';
import { PostsRoutingModule } from './post-routing.module';
import { PostTagEditorComponent } from './post-tag-editor/post-tag-editor.component';
import { PostTagPreviewComponent } from './post-tag-preview/post-tag-preview.component';
import { PostComponent } from './post/post.component';

@NgModule({
  declarations: [
    PostComponent,
    PostPreviewComponent,
    PostEditorComponent,
    PostTagPreviewComponent,
    PostFormComponent,
    PostCreatorComponent,
    PostTagEditorComponent,
    PostChangedAlertComponent,
  ],
  imports: [
    //angualr
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    //angular material
    MatListModule,
    MatSnackBarModule,

    //app
    ProfileModule,

    //routing
    PostsRoutingModule,
  ],
  exports: [
    PostPreviewComponent,
  ],
})
export class PostModule { }
