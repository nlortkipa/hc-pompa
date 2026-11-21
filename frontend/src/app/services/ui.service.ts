import { Injectable, signal } from '@angular/core';

export type ToastTone = 'success' | 'error' | 'info';

export interface Toast {
  id: number;
  message: string;
  tone: ToastTone;
}

export interface DialogOptions {
  title: string;
  message?: string;
  confirmLabel?: string;
  cancelLabel?: string | null;
  destructive?: boolean;
  tone?: 'warning' | 'danger' | 'info';
}

interface ActiveDialog extends DialogOptions {
  resolve: (confirmed: boolean) => void;
}

/**
 * App-wide feedback: transient toasts and macOS-style alert sheets that
 * replace the browser's blocking alert()/confirm().
 */
@Injectable({ providedIn: 'root' })
export class UiService {
  readonly toasts = signal<Toast[]>([]);
  readonly dialog = signal<ActiveDialog | null>(null);
  private nextId = 1;

  toast(message: string, tone: ToastTone = 'success', duration = 2800): void {
    const id = this.nextId++;
    this.toasts.update(list => [...list.slice(-2), { id, message, tone }]);
    setTimeout(() => this.dismissToast(id), duration);
  }

  dismissToast(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  confirm(options: DialogOptions): Promise<boolean> {
    this.dialog()?.resolve(false);
    return new Promise(resolve => {
      this.dialog.set({
        confirmLabel: 'დადასტურება',
        cancelLabel: 'გაუქმება',
        tone: options.destructive ? 'danger' : 'warning',
        ...options,
        resolve,
      });
    });
  }

  alert(title: string, message?: string): Promise<boolean> {
    return this.confirm({ title, message, confirmLabel: 'კარგი', cancelLabel: null, tone: 'info' });
  }

  resolveDialog(confirmed: boolean): void {
    const active = this.dialog();
    if (!active) return;
    this.dialog.set(null);
    active.resolve(confirmed);
  }
}
