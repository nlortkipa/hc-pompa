import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs';
import { PumpOperationReviewModel } from '../../models/pump-operation';
import { PumpOperationService } from '../../services/pump-operation-service';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review.html',
  styleUrls: ['./review.scss'],
})
export class Review implements OnInit {
  operation = signal<PumpOperationReviewModel | null>(null);
  isLoading = true;
  isSubmitting = false;
  error = '';
  message = '';
  private token = '';

  constructor(
    private route: ActivatedRoute,
    private pumpOperationService: PumpOperationService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token') || '';
    if (!this.token) {
      this.error = 'შემოწმების ბმული არასწორია.';
      this.isLoading = false;
      return;
    }

    this.loadReview();
  }

  loadReview(): void {
    this.isLoading = true;
    this.error = '';

    this.pumpOperationService.getReview(this.token)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => this.operation.set(data),
        error: () => this.error = 'ჩანაწერი ვერ მოიძებნა ან ბმული აღარ არის აქტიური.'
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
        error: () => this.error = 'გადაწყვეტილების შენახვა ვერ მოხერხდა.'
      });
  }
}
