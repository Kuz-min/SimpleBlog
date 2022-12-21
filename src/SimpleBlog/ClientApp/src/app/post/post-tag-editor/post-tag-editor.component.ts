import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { PostTag, PostTagService } from 'simple-blog/core';

@Component({
  selector: 'post-tag-editor',
  templateUrl: './post-tag-editor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostTagEditorComponent implements OnInit {

  tags: (Observable<PostTag[] | null> | null) = null;

  constructor(
    private readonly _postTagService: PostTagService,
  ) { }

  ngOnInit(): void {
    this.tags = this._postTagService.getAllAsync();
  }

  createTag(input: HTMLInputElement): void {
    this._postTagService.createAsync({ title: input.value }).subscribe(
      next => input.value = '',
    );
  }

  updateTag(tag: PostTag, title: string): void {
    this._postTagService.updateAsync(tag.id, { title: title }).subscribe();
  }

  deleteTag(tag: PostTag): void {
    this._postTagService.deleteAsync(tag.id).subscribe();
  }

}
