import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';
import { Profile, ProfileService } from 'simple-blog/core';

@Component({
  selector: 'profile-preview',
  templateUrl: './profile-preview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfilePreviewComponent implements OnInit {

  @Input() profileId: (string | null | undefined);

  profile: (Observable<Profile | null> | null) = null;

  constructor(
    private readonly _profileService: ProfileService,
  ) { }

  ngOnInit(): void {

    if (this.profileId) {
      this.profile = this._profileService.getByIdAsync(this.profileId).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of(null) : throwError(error)),
      );
    }

  }

}
