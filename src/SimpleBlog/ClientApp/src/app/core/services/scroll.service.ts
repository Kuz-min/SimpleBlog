import { Inject, Injectable } from '@angular/core';

@Injectable()
export class ScrollService {

  constructor(
    @Inject('DOCUMENT_ROOT') private readonly _documentRoot: HTMLElement,
  ) { }

  public lockScroll() {
    this._documentRoot.style.overflow = 'hidden';
  }

  public unlockScroll() {
    this._documentRoot.style.overflow = 'auto';
  }

}
