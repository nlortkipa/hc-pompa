import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { PumpOperationReviewModel } from '../models/pump-operation';
import { PumpOperationService } from './pump-operation-service';

/**
 * Review requests assigned to the signed-in user. Shared between the console
 * nav (pending badge) and the reviews page so a decision updates both.
 */
@Injectable({ providedIn: 'root' })
export class ReviewInboxService {
  private pumpOperationService = inject(PumpOperationService);

  readonly reviews = signal<PumpOperationReviewModel[]>([]);
  readonly loaded = signal(false);
  readonly pendingCount = computed(() => this.reviews().filter(r => r.reviewStatus === 'Pending').length);

  refresh(): Observable<PumpOperationReviewModel[]> {
    return this.pumpOperationService.getMyReviews().pipe(
      tap(reviews => {
        this.reviews.set(reviews);
        this.loaded.set(true);
      })
    );
  }

  /** Replace one review after a decision so the badge and list stay in sync. */
  update(review: PumpOperationReviewModel): void {
    this.reviews.update(list => list.map(r => r.token === review.token ? review : r));
  }

  clear(): void {
    this.reviews.set([]);
    this.loaded.set(false);
  }
}
