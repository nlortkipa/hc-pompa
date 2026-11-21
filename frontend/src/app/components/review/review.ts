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

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [CommonModule, RouterLink, Icon],
  templateUrl: './review.html',
  styleUrls: ['./review.scss'],
})
export class Review implements OnInit {
  operation = signal<PumpOperationReviewModel | null>(null);
  isLoading = true;
  isSubmitting = false;
  pendingDecision: 'accept' | 'reject' | null = null;
  isInvalidReview = false;
  canRetry = false;
  error = '';
  message = '';
  /** The operator edited the record while this page was open. */
  edited = signal(false);
  private token = '';
  private isRefreshing = false;
  private destroyRef = inject(DestroyRef);

  constructor(
    private route: ActivatedRoute,
    private pumpOperationService: PumpOperationService,
    private ui: UiService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token') || '';
    if (!/^[a-f0-9]{32}$/i.test(this.token)) {
      this.isInvalidReview = true;
      this.isLoading = false;
      return;
    }

    this.loadReview();
    livePoll(this.destroyRef, () => this.refreshLive());
  }

  /** Silent background refresh: picks up edits/deletion by the operator or another checker's decision. */
  private refreshLive(): void {
    const current = this.operation();
    if (!current || current.reviewStatus !== 'Pending' || this.isSubmitting || this.isRefreshing) return;

    this.isRefreshing = true;
    this.pumpOperationService.getReview(this.token)
      .pipe(finalize(() => this.isRefreshing = false))
      .subscribe({
        next: (data) => this.applyLiveUpdate(data),
        error: (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.isInvalidReview = true;
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
    if (this.isSubmitting || !previous || previous.reviewStatus !== 'Pending') return;

    this.operation.set(data);
    if (data.reviewStatus === 'Pending' && data.version !== previous.version) {
      this.markEdited();
    }
  }

  private markEdited(): void {
    this.edited.set(true);
    this.ui.toast('ოპერატორმა ჩანაწერი შეცვალა', 'info', 4000);
  }

  loadReview(): void {
    this.isLoading = true;
    this.isInvalidReview = false;
    this.canRetry = false;
    this.error = '';
    this.operation.set(null);

    this.pumpOperationService.getReview(this.token)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => this.operation.set(data),
        error: (error: HttpErrorResponse) => {
          if (error.status === 400 || error.status === 404) {
            this.isInvalidReview = true;
            return;
          }

          this.canRetry = true;
          this.error = 'მონაცემების ჩატვირთვა ვერ მოხერხდა. გთხოვთ, სცადოთ თავიდან.';
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
    if (this.isSubmitting || this.operation()?.reviewStatus !== 'Pending') return;

    this.isSubmitting = true;
    this.pendingDecision = decision;
    this.message = '';
    this.error = '';

    const version = this.operation()?.version;
    const request = decision === 'accept'
      ? this.pumpOperationService.acceptReview(this.token, version)
      : this.pumpOperationService.rejectReview(this.token, version);

    request.pipe(finalize(() => { this.isSubmitting = false; this.pendingDecision = null; }))
      .subscribe({
        next: (data) => {
          this.operation.set(data);
          this.edited.set(false);
          this.message = decision === 'accept'
            ? 'ჩანაწერი დადასტურდა.'
            : 'ჩანაწერი უარყოფილია.';
        },
        error: (error: HttpErrorResponse) => {
          if (error.status === 409 && error.error?.token) {
            // Edited after we loaded it: show the new data and let the checker decide again.
            this.operation.set(error.error as PumpOperationReviewModel);
            this.edited.set(true);
            this.ui.toast('ჩანაწერი შეიცვალა. გადახედეთ და გადაწყვიტეთ ხელახლა.', 'error', 5000);
            return;
          }

          if (error.status === 400 || error.status === 404) {
            this.isInvalidReview = true;
            this.operation.set(null);
            return;
          }

          this.error = 'გადაწყვეტილების შენახვა ვერ მოხერხდა.';
        }
      });
  }
}
