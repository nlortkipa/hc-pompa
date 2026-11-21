import { DestroyRef } from '@angular/core';
import { fromEvent, interval, merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

/** How often open review screens re-fetch to pick up edits by the operator. */
export const LIVE_POLL_MS = 10_000;

/**
 * Calls `tick` every LIVE_POLL_MS while the tab is visible, and immediately when the
 * tab becomes visible again. Stops when `destroyRef` is destroyed.
 */
export function livePoll(destroyRef: DestroyRef, tick: () => void): void {
  merge(
    interval(LIVE_POLL_MS),
    fromEvent(document, 'visibilitychange'),
  )
    .pipe(
      filter(() => document.visibilityState === 'visible'),
      takeUntilDestroyed(destroyRef),
    )
    .subscribe(() => tick());
}
