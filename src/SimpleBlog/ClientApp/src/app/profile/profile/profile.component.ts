import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, filter, map, mergeMap, Observable, of, throwError } from 'rxjs';
import { Profile, ProfileService } from 'simple-blog/core';

@Component({
  selector: 'profile',
  templateUrl: './profile.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileComponent implements OnInit {

  profile: (Observable<Profile | null> | null) = null;

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _profileService: ProfileService,
  ) { }

  ngOnInit(): void {
    this.profile = this._route.params.pipe(
      filter(params => params['id']),
      map(params => Number(params['id'])),
      mergeMap(id => this._profileService.getByIdAsync(id).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of(null) : throwError(error)),
      )),
    );
  }

}
