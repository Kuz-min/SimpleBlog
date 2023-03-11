import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { Post } from 'simple-blog/core';

@Component({
  selector: 'app-post-changed-alert',
  templateUrl: './post-changed-alert.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostChangedAlertComponent {

  constructor(
    private readonly _snackBarRef: MatSnackBarRef<PostChangedAlertComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public readonly data: { message: string, post?: Post | null },
  ) { }

  close(): void {
    this._snackBarRef.dismiss();
  }

}
