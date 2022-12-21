import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthenticationService, ScrollService } from 'simple-blog/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavMenuComponent implements OnInit {

  isAuthenticated: (Observable<boolean> | null) = null;

  readonly mainMenu = new BehaviorSubject<{ isExpanded: boolean }>({ isExpanded: false });
  readonly profileMenu = new BehaviorSubject<{ isExpanded: boolean }>({ isExpanded: false });

  constructor(
    private readonly _scrollService: ScrollService,
    private readonly _authService: AuthenticationService,
  ) { }

  ngOnInit(): void {

    this.isAuthenticated = this._authService.isAuthenticatedAsync();

    this.mainMenu.subscribe(
      next => {
        if (next.isExpanded) {
          this._scrollService.lockScroll();
          this._scrollService.scrollToPosition([0, 0]);
        } else {
          this._scrollService.unlockScroll();
        }
      });

  }

  toggleMainMenu(): void {
    this.mainMenu.next({ isExpanded: !this.mainMenu.value.isExpanded });
  }

  collapseMainMenu(): void {
    this.mainMenu.next({ isExpanded: false });
  }

  toggleProfileMenu(): void {
    this.profileMenu.next({ isExpanded: !this.profileMenu.value.isExpanded });
  }

  collapseProfileMenu(): void {
    this.profileMenu.next({ isExpanded: false });
  }

}
