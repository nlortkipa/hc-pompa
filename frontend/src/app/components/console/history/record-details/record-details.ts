import { AfterViewInit, Component, ElementRef, HostListener, OnDestroy, computed, input, output, viewChild } from '@angular/core';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { PumpOperationModel } from '../../../../models/pump-operation';

@Component({
  selector: 'app-record-details',
  standalone: true,
  imports: [DatePipe, DecimalPipe, NgClass],
  templateUrl: './record-details.html',
  styleUrls: ['./record-details.scss'],
})
export class RecordDetails implements AfterViewInit, OnDestroy {
  record = input.required<PumpOperationModel>();
  showOperatorDetails = input(false);
  closed = output<void>();

  private closeButton = viewChild<ElementRef<HTMLButtonElement>>('closeButton');
  private previousBodyOverflow = '';

  note = computed(() => this.record().note?.trim() || '');

  status = computed(() => {
    const status = this.record().reviewStatus || 'Pending';
    if (status === 'Accepted') return { label: 'დადასტურებული', cls: 'badge-green' };
    if (status === 'Rejected') return { label: 'უარყოფილი', cls: 'badge-red' };
    return { label: 'მოლოდინში', cls: 'badge-yellow' };
  });

  distanceRange = computed(() => {
    const r = this.record();
    if (r.pumpType !== '<60მ') return '—';
    return r.distanceRange || '60მ-მდე';
  });

  ngAfterViewInit(): void {
    this.previousBodyOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    this.closeButton()?.nativeElement.focus();
  }

  ngOnDestroy(): void {
    document.body.style.overflow = this.previousBodyOverflow;
  }

  @HostListener('document:keydown.escape')
  close(): void {
    this.closed.emit();
  }
}
