import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { Post } from 'simple-blog/core';

@Component({
  selector: 'post-preview',
  templateUrl: './post-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostPreviewComponent {

  @Input() post: (Post | null | undefined);

  constructor() { }

}
