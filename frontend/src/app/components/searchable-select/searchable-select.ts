import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, OnChanges, Output, SimpleChanges } from '@angular/core';

export interface SearchableSelectOption {
  value: number;
  label: string;
}

@Component({
  selector: 'app-searchable-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './searchable-select.html',
  styleUrls: ['./searchable-select.scss'],
})
export class SearchableSelect implements OnChanges {
  @Input() options: SearchableSelectOption[] = [];
  @Input() value: number | null = null;
  @Input() placeholder = 'მოძებნეთ';
  @Input() allLabel = 'ყველა';
  @Input() query = '';
  @Output() valueChange = new EventEmitter<number | null>();
  @Output() queryChange = new EventEmitter<string>();

  isOpen = false;

  constructor(private elementRef: ElementRef<HTMLElement>) {}

  ngOnChanges(changes: SimpleChanges): void {
    const valueChangedWithoutQuery = changes['value'] && this.value != null && !changes['query'];
    const selectedOptionsLoaded = changes['options'] && this.value != null && !this.query;

    if (valueChangedWithoutQuery || selectedOptionsLoaded) {
      this.syncQueryWithValue();
    }
  }

  get filteredOptions(): SearchableSelectOption[] {
    const selectedOption = this.options.find(option => option.value === this.value);
    const normalizedQuery = this.query.trim().toLocaleLowerCase();

    if (!normalizedQuery || selectedOption?.label === this.query) {
      return this.options;
    }

    return this.options.filter(option =>
      option.label.toLocaleLowerCase().includes(normalizedQuery)
    );
  }

  onInput(event: Event): void {
    this.query = (event.target as HTMLInputElement).value;
    this.isOpen = true;

    if (this.value != null) {
      this.value = null;
      this.valueChange.emit(null);
    }

    this.queryChange.emit(this.query);
  }

  open(): void {
    this.isOpen = true;
  }

  toggle(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isOpen = !this.isOpen;
  }

  select(option: SearchableSelectOption): void {
    this.value = option.value;
    this.query = option.label;
    this.isOpen = false;
    this.valueChange.emit(option.value);
    this.queryChange.emit(option.label);
  }

  clear(event?: MouseEvent): void {
    event?.preventDefault();
    event?.stopPropagation();
    this.value = null;
    this.query = '';
    this.isOpen = false;
    this.valueChange.emit(null);
    this.queryChange.emit('');
  }

  closeOnEscape(): void {
    this.isOpen = false;
  }

  @HostListener('document:click', ['$event.target'])
  onDocumentClick(target: EventTarget | null): void {
    if (!target || !this.elementRef.nativeElement.contains(target as Node)) {
      this.isOpen = false;
    }
  }

  private syncQueryWithValue(): void {
    this.query = this.options.find(option => option.value === this.value)?.label ?? '';
  }
}
