import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { BehaviorSubject, Observable, of, switchMap } from 'rxjs';
import { catchNotFound, filterNotNullAndNotUndefined, Profile, ProfileService } from 'simple-blog/core';

@Component({
  selector: 'profile-preview',
  templateUrl: './profile-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfilePreviewComponent {

  @Input() set profileId(id: string | null | undefined) {
    this._id.next(id);
  }

  readonly profile: Observable<Profile | null>;

  constructor(
    private readonly _profileService: ProfileService,
  ) {

    this.profile = this._id.pipe(
      filterNotNullAndNotUndefined(),
      switchMap(id => this._profileService.getByIdAsync(id).pipe(
        catchNotFound(of(null)),
      )),
    );

  }

  private readonly _id = new BehaviorSubject<string | null | undefined>(null);

}
