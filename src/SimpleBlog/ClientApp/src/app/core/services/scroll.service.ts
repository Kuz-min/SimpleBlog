import { ViewportScroller } from '@angular/common';
import { Inject, Injectable } from '@angular/core';

@Injectable()
export class ScrollService {

  constructor(
    @Inject('DOCUMENT_ROOT') private readonly _documentRoot: HTMLElement,
    private readonly _viewportScroller: ViewportScroller,
  ) { }

  public lockScroll(): void {
    this._documentRoot.style.overflow = 'hidden';
  }

  public unlockScroll(): void {
    this._documentRoot.style.overflow = 'auto';
  }

  public scrollToPosition(position: [number, number]): void {
    this._viewportScroller.scrollToPosition(position);
  }

}
