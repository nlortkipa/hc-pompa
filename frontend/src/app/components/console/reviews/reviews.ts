import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { PumpOperationReviewModel } from '../../../models/pump-operation';
import { PumpOperationService } from '../../../services/pump-operation-service';
import { ReviewInboxService } from '../../../services/review-inbox.service';
import { UiService } from '../../../services/ui.service';
import { Icon } from '../../icon/icon';
import { livePoll } from '../../../services/live-poll';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule, RouterLink, Icon],
  templateUrl: './reviews.html',
  styleUrls: ['./reviews.scss'],
})
export class Reviews implements OnInit {
  private inbox = inject(ReviewInboxService);
  private pumpOperationService = inject(PumpOperationService);
  private ui = inject(UiService);
  private destroyRef = inject(DestroyRef);
  private isRefreshing = false;

  /** Pending reviews the operator edited while this list was open. */
  editedTokens = signal<ReadonlySet<string>>(new Set());

  filter = signal<'pending' | 'all'>('pending');
  isLoading = false;
  submittingToken: string | null = null;

  reviews = this.inbox.reviews;
  loaded = this.inbox.loaded;
  pendingCount = this.inbox.pendingCount;
  visibleReviews = computed(() =>
    this.filter() === 'pending'
      ? this.reviews().filter(r => r.reviewStatus === 'Pending')
      : this.reviews()
  );

  ngOnInit(): void {
    this.load();
    livePoll(this.destroyRef, () => this.refreshLive());
  }

  /** Silent background refresh that flags and announces edits by operators. */
  private refreshLive(): void {
    if (this.isLoading || this.isRefreshing || this.submittingToken) return;

    const before = new Map(this.reviews().map(r => [r.token, r]));
    this.isRefreshing = true;
    this.inbox.refresh().pipe(finalize(() => this.isRefreshing = false)).subscribe({
      next: (list) => {
        const edited = list.filter(r => {
          const old = before.get(r.token);
          return r.reviewStatus === 'Pending' && old?.reviewStatus === 'Pending' && old.version !== r.version;
        });
        if (edited.length === 0) return;

        this.editedTokens.update(set => new Set([...set, ...edited.map(r => r.token)]));
        this.ui.toast(
          edited.length === 1
            ? `ოპერატორმა ჩანაწერი შეცვალა (${edited[0].plateNumber})`
            : `ოპერატორებმა ${edited.length} ჩანაწერი შეცვალეს`,
          'info',
          4000
        );
      },
      // Keep the current list; the next tick retries.
      error: () => {},
    });
  }

  isEdited(review: PumpOperationReviewModel): boolean {
    return review.reviewStatus === 'Pending' && this.editedTokens().has(review.token);
  }

  load(): void {
    this.isLoading = true;
    this.inbox.refresh().pipe(finalize(() => this.isLoading = false)).subscribe({
      error: () => this.ui.toast('რეპორტების ჩატვირთვა ვერ მოხერხდა', 'error'),
    });
  }

  accept(review: PumpOperationReviewModel): void {
    this.submit(review, 'accept');
  }

  async reject(review: PumpOperationReviewModel): Promise<void> {
    const ok = await this.ui.confirm({
      title: 'უარვყოთ ჩანაწერი?',
      message: 'გადაწყვეტილების შეცვლა შემდეგ ვეღარ მოხერხდება.',
      confirmLabel: 'უარყოფა',
      destructive: true,
    });
    if (ok) this.submit(review, 'reject');
  }

  statusClass(status?: string): string {
    if (status === 'Accepted') return 'badge-green';
    if (status === 'Rejected') return 'badge-red';
    return 'badge-yellow';
  }

  statusLabel(status?: string): string {
    if (status === 'Accepted') return 'დადასტურებული';
    if (status === 'Rejected') return 'უარყოფილი';
    return 'მოლოდინში';
  }

  private submit(review: PumpOperationReviewModel, decision: 'accept' | 'reject'): void {
    if (this.submittingToken) return;
    this.submittingToken = review.token;

    const request = decision === 'accept'
      ? this.pumpOperationService.acceptReview(review.token, review.version)
      : this.pumpOperationService.rejectReview(review.token, review.version);

    request.pipe(finalize(() => this.submittingToken = null)).subscribe({
      next: (updated) => {
        this.inbox.update(updated);
        if (updated.reviewStatus === (decision === 'accept' ? 'Accepted' : 'Rejected')) {
          this.ui.toast(decision === 'accept' ? 'ჩანაწერი დადასტურდა' : 'ჩანაწერი უარყოფილია');
        } else {
          // Another checker decided first; show their outcome.
          this.ui.toast(`ჩანაწერი უკვე ${this.statusLabel(updated.reviewStatus).toLowerCase()} იყო`, 'info');
        }
      },
      error: (error: HttpErrorResponse) => {
        if (error.status === 409 && error.error?.token) {
          // Edited since the list loaded: show the new data and let the checker decide again.
          this.inbox.update(error.error as PumpOperationReviewModel);
          this.editedTokens.update(set => new Set([...set, review.token]));
          this.ui.toast('ჩანაწერი შეიცვალა. გადახედეთ და გადაწყვიტეთ ხელახლა.', 'error', 5000);
          return;
        }
        if (error.status === 404) {
          this.ui.toast('ჩანაწერი აღარ არსებობს', 'error');
          this.load();
          return;
        }
        this.ui.toast('გადაწყვეტილების შენახვა ვერ მოხერხდა', 'error');
      },
    });
  }
}
