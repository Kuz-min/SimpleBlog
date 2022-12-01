import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PostService } from 'simple-blog/core';

@Component({
  selector: 'post-creator',
  templateUrl: './post-creator.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostCreatorComponent implements OnInit {

  lockForm = new BehaviorSubject<boolean>(false);
  isCreated = new BehaviorSubject<boolean>(false);

  constructor(
    private readonly _postService: PostService,
  ) { }

  ngOnInit(): void {
  }

  onComplited(data: { title: string, content: string, tagIds?: number[] }): void {
    this.lockForm.next(true);
    this._postService.createAsync({ title: data.title, content: data.content, tagIds: data.tagIds }).subscribe(
      next => {
        this.lockForm.next(false);
        this.isCreated.next(true);
      },
      error => {
        console.error(error);
        this.lockForm.next(false);
      },
    );
  }

}
