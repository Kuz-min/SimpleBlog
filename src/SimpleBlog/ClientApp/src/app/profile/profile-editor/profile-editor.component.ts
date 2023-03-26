import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, catchError, filter, first, map, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { Profile, ProfileService } from 'simple-blog/core';

@Component({
  selector: 'profile-editor',
  templateUrl: './profile-editor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileEditorComponent implements OnInit {

  profile: (Observable<Profile | null> | null) = null;

  readonly isImageUpdated = new BehaviorSubject<boolean>(false);

  readonly imagePath = new BehaviorSubject<string | null>(null);
  readonly imageForm = new FormGroup({
    file: new FormControl(null, [Validators.required]),
  });

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _profileService: ProfileService,
  ) { }

  ngOnInit(): void {
    this.profile = this._route.params.pipe(
      filter(params => params['id']),
      map(params => params['id'] as string),
      tap(id => this._profileId = id),
      switchMap(id => this._profileService.getByIdAsync(id).pipe(
        catchError(error => (error as HttpErrorResponse)?.status == 404 ? of(null) : throwError(error)),
      )),
    );
  }

  updateImage(): void {
    if (!this._profileId || !this._imageFile)
      return;

    this._profileService.updateImageAsync(this._profileId, this._imageFile).pipe(
      first(),
      tap(() => this.isImageUpdated.next(true)),
    ).subscribe();
  }

  imagePreview(event: any): void {
    const file = (event.target as HTMLInputElement)?.files?.[0];

    if (!file)
      return;

    this._imageFile = file;

    const reader = new FileReader();
    reader.onload = () => this.imagePath.next(reader.result as string);
    reader.readAsDataURL(file);
  }

  private _profileId: (string | null) = null;
  private _imageFile: (File | null) = null;

}
