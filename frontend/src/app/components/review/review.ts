import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { PumpOperationReviewModel } from '../../models/pump-operation';
import { PumpOperationService } from '../../services/pump-operation-service';
import { UiService } from '../../services/ui.service';
import { Icon } from '../icon/icon';
import { livePoll } from '../../services/live-poll';
import { ReviewInboxService } from '../../services/review-inbox.service';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [CommonModule, RouterLink, Icon],
  templateUrl: './review.html',
  styleUrls: ['./review.scss'],
})
export class Review implements OnInit {
  operation = signal<PumpOperationReviewModel | null>(null);
  isLoading = signal(true);
  isSubmitting = signal(false);
  pendingDecision = signal<'accept' | 'reject' | null>(null);
  isInvalidReview = signal(false);
  /** The link was valid but the operator deleted the report. */
  isDeleted = signal(false);
  canRetry = signal(false);
  error = signal('');
  message = signal('');
  private token = '';
  private isRefreshing = false;
  private destroyRef = inject(DestroyRef);
  private inbox = inject(ReviewInboxService);
  /** Opened from inside the console (sidebar/header around it) rather than from an email link. */
  embedded = false;

  constructor(
    private route: ActivatedRoute,
    private pumpOperationService: PumpOperationService,
    private ui: UiService
  ) {}

  ngOnInit(): void {
    this.embedded = !!this.route.snapshot.data['embedded'];
    this.token = this.route.snapshot.paramMap.get('token') || '';
    if (!/^[a-f0-9]{32}$/i.test(this.token)) {
      this.isInvalidReview.set(true);
      this.isLoading.set(false);
      return;
    }

    this.loadReview();
    livePoll(this.destroyRef, () => this.refreshLive());
  }

  /** Silent background refresh: picks up edits/deletion by the operator or another checker's decision. */
  private refreshLive(): void {
    const current = this.operation();
    if (!current || current.reviewStatus !== 'Pending' || this.isSubmitting() || this.isRefreshing) return;

    this.isRefreshing = true;
    this.pumpOperationService.getReview(this.token)
      .pipe(finalize(() => this.isRefreshing = false))
      .subscribe({
        next: (data) => this.applyLiveUpdate(data),
        error: (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.isDeleted.set(true);
            this.operation.set(null);
            this.ui.toast('ოპერატორმა ჩანაწერი წაშალა', 'info');
          }
          // Other errors: keep showing what we have and try again on the next tick.
        }
      });
  }

  private applyLiveUpdate(data: PumpOperationReviewModel): void {
    const previous = this.operation();
    // Ignore a response that raced with a decision made on this page.
    if (this.isSubmitting() || !previous || previous.reviewStatus !== 'Pending') return;

    this.operation.set(data);
    this.syncInbox(data);
    if (data.reviewStatus === 'Pending' && data.version !== previous.version) {
      this.ui.toast('ოპერატორმა ჩანაწერი შეცვალა', 'info', 4000);
    }
  }

  /** Keep the console's pending badge and list in step with what this page shows. */
  private syncInbox(data: PumpOperationReviewModel): void {
    if (this.embedded) this.inbox.update(data);
  }

  loadReview(): void {
    this.isLoading.set(true);
    this.isInvalidReview.set(false);
    this.isDeleted.set(false);
    this.canRetry.set(false);
    this.error.set('');
    this.operation.set(null);

    this.pumpOperationService.getReview(this.token)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => this.operation.set(data),
        error: (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.isDeleted.set(true);
            return;
          }
          if (error.status === 400) {
            this.isInvalidReview.set(true);
            return;
          }

          this.canRetry.set(true);
          this.error.set('მონაცემების ჩატვირთვა ვერ მოხერხდა. გთხოვთ, სცადოთ თავიდან.');
        }
      });
  }

  accept(): void {
    this.submitDecision('accept');
  }

  async reject(): Promise<void> {
    const ok = await this.ui.confirm({
      title: 'უარვყოთ ჩანაწერი?',
      message: 'გადაწყვეტილების შეცვლა შემდეგ ვეღარ მოხერხდება.',
      confirmLabel: 'უარყოფა',
      destructive: true,
    });
    if (ok) this.submitDecision('reject');
  }

  statusClass(status?: string): string {
    if (status === 'Accepted') return 'badge-green';
    if (status === 'Rejected') return 'badge-red';
    return 'badge-yellow';
  }

  statusLabel(status?: string): string {
    if (status === 'Accepted') return 'დადასტურებულია';
    if (status === 'Rejected') return 'უარყოფილია';
    return 'მოლოდინში';
  }

  private submitDecision(decision: 'accept' | 'reject'): void {
    if (this.isSubmitting() || this.operation()?.reviewStatus !== 'Pending') return;

    this.isSubmitting.set(true);
    this.pendingDecision.set(decision);
    this.message.set('');
    this.error.set('');

    const version = this.operation()?.version;
    const request = decision === 'accept'
      ? this.pumpOperationService.acceptReview(this.token, version)
      : this.pumpOperationService.rejectReview(this.token, version);

    request.pipe(finalize(() => { this.isSubmitting.set(false); this.pendingDecision.set(null); }))
      .subscribe({
        next: (data) => {
          this.operation.set(data);
          this.syncInbox(data);
          this.message.set(decision === 'accept'
            ? 'ჩანაწერი დადასტურდა.'
            : 'ჩანაწერი უარყოფილია.');
        },
        error: (error: HttpErrorResponse) => {
          if (error.status === 409 && error.error?.token) {
            // Edited after we loaded it: show the new data and let the checker decide again.
            this.operation.set(error.error as PumpOperationReviewModel);
            this.syncInbox(error.error as PumpOperationReviewModel);
            this.ui.toast('ჩანაწერი შეიცვალა. გადახედეთ და გადაწყვიტეთ ხელახლა.', 'error', 5000);
            return;
          }

          if (error.status === 404) {
            this.isDeleted.set(true);
            this.operation.set(null);
            return;
          }
          if (error.status === 400) {
            this.isInvalidReview.set(true);
            this.operation.set(null);
            return;
          }

          this.error.set('გადაწყვეტილების შენახვა ვერ მოხერხდა.');
        }
      });
  }
}
