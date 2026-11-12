import { EventEmitter, Injectable, OnDestroy, effect, inject, signal } from '@angular/core';
import { Direction, Directionality } from '@angular/cdk/bidi';
import { TranslationService } from './translation.service';

@Injectable({
  providedIn: 'root'
})
export class AppDirectionality implements Directionality, OnDestroy {
  private readonly i18n = inject(TranslationService);

  readonly valueSignal = signal<Direction>(this.i18n.dir());
  readonly change = new EventEmitter<Direction>();

  get value(): Direction {
    return this.valueSignal();
  }

  constructor() {
    effect(() => {
      const currentDir = this.i18n.dir();
      this.valueSignal.set(currentDir);
      this.change.emit(currentDir);
    });
  }

  ngOnDestroy(): void {
    this.change.complete();
  }
}
