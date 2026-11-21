import { CommonModule } from '@angular/common';
import { Icon } from '../icon/icon';
import { Component, ElementRef, EventEmitter, HostListener, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';

export interface SearchableSelectOption {
  value: number;
  label: string;
}

@Component({
  selector: 'app-searchable-select',
  standalone: true,
  imports: [CommonModule, Icon],
  templateUrl: './searchable-select.html',
  styleUrls: ['./searchable-select.scss'],
})
export class SearchableSelect implements OnInit, OnChanges, OnDestroy {
  @Input() options: SearchableSelectOption[] = [];
  @Input() value: number | null = null;
  @Input() placeholder = 'მოძებნეთ';
  @Input() allLabel = 'ყველა';
  @Input() query = '';
  /** Show the leading "all" row that clears the value (filter usage). */
  @Input() showAll = true;
  /**
   * Render the menu fixed to the viewport so it can extend past a clipping
   * container (e.g. a scrolling sheet). Flips upward when space below is short.
   */
  @Input() floating = false;
  @Output() valueChange = new EventEmitter<number | null>();
  @Output() queryChange = new EventEmitter<string>();

  isOpen = false;
  menuStyle: Record<string, string> | null = null;
  private readonly onViewportChange = () => { if (this.isOpen && this.floating) this.positionMenu(); };
  /** Keyboard highlight: -1 = "all" row, 0..n = filtered options. */
  activeIndex = -1;

  constructor(private elementRef: ElementRef<HTMLElement>) {}

  ngOnInit(): void {
    if (!this.floating) return;
    // Capture phase so scrolling any ancestor (the sheet body) repositions the menu.
    window.addEventListener('scroll', this.onViewportChange, true);
    window.addEventListener('resize', this.onViewportChange);
  }

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.onViewportChange, true);
    window.removeEventListener('resize', this.onViewportChange);
  }

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
    this.open();
    this.activeIndex = this.query || !this.showAll ? 0 : -1;

    if (this.value != null) {
      this.value = null;
      this.valueChange.emit(null);
    }

    this.queryChange.emit(this.query);
  }

  open(): void {
    this.isOpen = true;
    if (!this.showAll && this.activeIndex < 0) this.activeIndex = 0;
    if (this.floating) this.positionMenu();
  }

  private positionMenu(): void {
    const control = this.elementRef.nativeElement.querySelector('.search-select__control');
    if (!control) return;

    const rect = control.getBoundingClientRect();
    const gap = 4;
    const margin = 12;
    const spaceBelow = window.innerHeight - rect.bottom - gap - margin;
    const spaceAbove = rect.top - gap - margin;
    const openUp = spaceBelow < 200 && spaceAbove > spaceBelow;
    const maxHeight = Math.max(120, Math.min(280, openUp ? spaceAbove : spaceBelow));

    this.menuStyle = {
      position: 'fixed',
      left: `${rect.left}px`,
      width: `${rect.width}px`,
      'max-height': `${maxHeight}px`,
      top: openUp ? 'auto' : `${rect.bottom + gap}px`,
      bottom: openUp ? `${window.innerHeight - rect.top + gap}px` : 'auto',
    };
  }

  toggle(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    if (this.isOpen) this.isOpen = false;
    else this.open();
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

  onKeydown(event: KeyboardEvent): void {
    const options = this.filteredOptions;
    if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
      event.preventDefault();
      if (!this.isOpen) { this.open(); return; }
      const delta = event.key === 'ArrowDown' ? 1 : -1;
      const minIndex = this.showAll ? -1 : 0;
      this.activeIndex = Math.max(minIndex, Math.min(options.length - 1, this.activeIndex + delta));
      queueMicrotask(() =>
        this.elementRef.nativeElement.querySelector('.search-select__option--active')?.scrollIntoView({ block: 'nearest' })
      );
    } else if (event.key === 'Enter' && this.isOpen) {
      event.preventDefault();
      if (this.activeIndex === -1) { if (this.showAll) this.clear(); }
      else if (options[this.activeIndex]) this.select(options[this.activeIndex]);
    } else if (event.key === 'Tab') {
      this.isOpen = false;
    }
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
