import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IsAuthenticatedGuard } from 'simple-blog/core';
import { PostCreatorComponent } from './post-creator/post-creator.component';
import { PostEditorComponent } from './post-editor/post-editor.component';
import { PostTagEditorComponent } from './post-tag-editor/post-tag-editor.component';
import { PostComponent } from './post/post.component';

const routes: Routes = [
  { path: 'posts/create', component: PostCreatorComponent, canActivate: [IsAuthenticatedGuard] },

  { path: 'posts/:id', component: PostComponent },
  { path: 'posts/:id/edit', component: PostEditorComponent, canActivate: [IsAuthenticatedGuard] },

  { path: 'post-tags/edit', component: PostTagEditorComponent, canActivate: [IsAuthenticatedGuard] },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
  ],
  exports: [
    RouterModule,
  ],
})
export class PostsRoutingModule { }
