import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { Post, PostService } from 'simple-blog/core';


@Component({
  selector: 'post-preview',
  templateUrl: './post-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostPreviewComponent implements OnInit {

  @Input() postId: (number | null | undefined);
  @Input() post: (Post | null | undefined);

  constructor(
    private readonly _postService: PostService,
  ) { }

  ngOnInit(): void {
  }

}
