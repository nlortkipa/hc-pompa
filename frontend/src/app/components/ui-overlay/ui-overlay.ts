import { Component, ElementRef, HostListener, effect, inject, viewChild } from '@angular/core';
import { UiService } from '../../services/ui.service';
import { Icon } from '../icon/icon';

@Component({
  selector: 'app-ui-overlay',
  standalone: true,
  imports: [Icon],
  template: `
    <div class="toast-stack" aria-live="polite">
      @for (t of ui.toasts(); track t.id) {
        <div class="toast-item" [class]="'toast-' + t.tone" role="status">
          <app-icon [name]="t.tone === 'error' ? 'x-circle' : t.tone === 'info' ? 'info' : 'check-circle'" [size]="18" />
          <span>{{ t.message }}</span>
        </div>
      }
    </div>

    @if (ui.dialog(); as d) {
      <div class="overlay-backdrop" (click)="ui.resolveDialog(false)">
        <div class="alert-sheet" role="alertdialog" aria-modal="true" aria-labelledby="alert-title"
          (click)="$event.stopPropagation()">
          <div class="alert-icon" [class]="'tone-' + d.tone">
            <app-icon [name]="d.tone === 'info' ? 'info' : 'alert'" [size]="26" [stroke]="1.6" />
          </div>
          <h2 id="alert-title">{{ d.title }}</h2>
          @if (d.message) { <p>{{ d.message }}</p> }
          <div class="alert-actions" [class.single]="!d.cancelLabel">
            @if (d.cancelLabel) {
              <button type="button" class="btn btn-secondary" (click)="ui.resolveDialog(false)">{{ d.cancelLabel }}</button>
            }
            <button #confirmBtn type="button" class="btn" [class.btn-danger]="d.destructive"
              [class.btn-primary]="!d.destructive" (click)="ui.resolveDialog(true)">{{ d.confirmLabel }}</button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .toast-stack {
      position: fixed; left: 50%; bottom: calc(24px + env(safe-area-inset-bottom)); z-index: 11000;
      display: grid; gap: 8px; justify-items: center; width: max-content; max-width: calc(100vw - 32px);
      transform: translateX(-50%); pointer-events: none;
    }
    /* Phones: sit above the bottom tab bar so it stays tappable. */
    @media (max-width: 768px) {
      .toast-stack { bottom: calc(84px + env(safe-area-inset-bottom)); }
    }
    .toast-item {
      display: flex; align-items: center; gap: 10px; padding: 10px 16px 10px 12px;
      background: var(--material-thick); color: var(--text);
      -webkit-backdrop-filter: saturate(180%) blur(24px); backdrop-filter: saturate(180%) blur(24px);
      border-radius: 999px; box-shadow: var(--shadow-popover);
      font-size: 13px; font-weight: 500; animation: toast-in 260ms cubic-bezier(.2,.9,.3,1.2);
    }
    .toast-success app-icon { color: var(--green); }
    .toast-error app-icon { color: var(--danger); }
    .toast-info app-icon { color: var(--blue); }
    .overlay-backdrop {
      position: fixed; inset: 0; z-index: 10500; display: grid; place-items: center; padding: 20px;
      background: var(--scrim); animation: fade 160ms ease-out;
    }
    .alert-sheet {
      width: min(300px, 100%); padding: 22px 18px 16px; text-align: center;
      background: var(--material-thick);
      -webkit-backdrop-filter: saturate(180%) blur(30px); backdrop-filter: saturate(180%) blur(30px);
      border-radius: 14px; box-shadow: var(--shadow-window);
      animation: pop 220ms cubic-bezier(.2,.9,.3,1.15);
    }
    .alert-icon { display: inline-flex; padding: 10px; margin-bottom: 10px; border-radius: 14px; }
    .tone-warning { color: var(--orange); background: var(--orange-soft); }
    .tone-danger { color: var(--danger); background: var(--danger-soft); }
    .tone-info { color: var(--blue); background: var(--blue-soft); }
    h2 { margin-bottom: 6px; font-size: 14px; font-weight: 700; line-height: 1.35; }
    p { color: var(--text-muted); font-size: 12.5px; line-height: 1.45; }
    .alert-actions { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; margin-top: 18px; }
    .alert-actions.single { grid-template-columns: 1fr; }
    .alert-actions .btn { width: 100%; white-space: normal; }
    @keyframes toast-in { from { opacity: 0; transform: translateY(12px) scale(.96); } }
    @keyframes fade { from { opacity: 0; } }
    @keyframes pop { from { opacity: 0; transform: scale(.92); } }
  `],
})
export class UiOverlay {
  ui = inject(UiService);
  private confirmBtn = viewChild<ElementRef<HTMLButtonElement>>('confirmBtn');

  constructor() {
    effect(() => {
      if (this.ui.dialog()) queueMicrotask(() => this.confirmBtn()?.nativeElement.focus());
    });
  }

  @HostListener('document:keydown.escape', ['$event'])
  onEscape(event: Event): void {
    if (!this.ui.dialog()) return;
    event.stopImmediatePropagation();
    this.ui.resolveDialog(false);
  }
}
