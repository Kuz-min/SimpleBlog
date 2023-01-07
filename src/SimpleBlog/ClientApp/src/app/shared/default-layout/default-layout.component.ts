import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { NavigationStart, Router } from '@angular/router';
import { distinctUntilChanged, filter, map, Subscription } from 'rxjs';

@Component({
  selector: 'shared-default-layout',
  templateUrl: './default-layout.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DefaultLayoutComponent implements OnInit, OnDestroy {

  @ViewChild('mainSidenav') mainSidenav: (MatDrawer | null) = null;
  @ViewChild('profileSidenav') profileSidenav: (MatDrawer | null) = null;

  constructor(
    private readonly _router: Router,
  ) { }

  ngOnInit(): void {
    this._routeTracker = this._router.events.pipe(
      filter(e => e instanceof NavigationStart),
      map(e => (e as NavigationStart).url.split('?')[0]),
      distinctUntilChanged(),
    ).subscribe(
      () => {
        this.mainSidenav?.close();
        this.profileSidenav?.close();
      },
    );
  }

  ngOnDestroy(): void {
    this._routeTracker?.unsubscribe();
  }

  private _routeTracker: (Subscription | null) = null;

}
