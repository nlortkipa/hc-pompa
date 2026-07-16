import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { PumpOperationReviewModel } from '../../models/pump-operation';
import { PumpOperationService } from '../../services/pump-operation-service';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './review.html',
  styleUrls: ['./review.scss'],
})
export class Review implements OnInit {
  operation = signal<PumpOperationReviewModel | null>(null);
  isLoading = true;
  isSubmitting = false;
  isInvalidReview = false;
  canRetry = false;
  error = '';
  message = '';
  private token = '';

  constructor(
    private route: ActivatedRoute,
    private pumpOperationService: PumpOperationService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token') || '';
    if (!/^[a-f0-9]{32}$/i.test(this.token)) {
      this.isInvalidReview = true;
      this.isLoading = false;
      return;
    }

    this.loadReview();
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

  reject(): void {
    this.submitDecision('reject');
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
    this.message = '';
    this.error = '';

    const request = decision === 'accept'
      ? this.pumpOperationService.acceptReview(this.token)
      : this.pumpOperationService.rejectReview(this.token);

    request.pipe(finalize(() => this.isSubmitting = false))
      .subscribe({
        next: (data) => {
          this.operation.set(data);
          this.message = decision === 'accept'
            ? 'ჩანაწერი დადასტურდა.'
            : 'ჩანაწერი უარყოფილია.';
        },
        error: (error: HttpErrorResponse) => {
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
