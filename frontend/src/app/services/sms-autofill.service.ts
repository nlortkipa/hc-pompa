import { Injectable } from '@angular/core';
import { Capacitor, PluginListenerHandle, registerPlugin } from '@capacitor/core';
import { BehaviorSubject } from 'rxjs';

interface SmsCodeReceivedEvent {
  code: string;
}

interface SmsAutofillPlugin {
  startListening(): Promise<void>;
  addListener(
    eventName: 'codeReceived',
    listener: (event: SmsCodeReceivedEvent) => void,
  ): Promise<PluginListenerHandle>;
}

const SmsAutofill = registerPlugin<SmsAutofillPlugin>('SmsAutofill');

@Injectable({ providedIn: 'root' })
export class SmsAutofillService {
  private readonly codeSubject = new BehaviorSubject<string | null>(null);
  private listener?: PluginListenerHandle;

  readonly code$ = this.codeSubject.asObservable();

  async startListening(): Promise<void> {
    this.codeSubject.next(null);

    if (!Capacitor.isNativePlatform() || Capacitor.getPlatform() !== 'android') {
      return;
    }

    try {
      this.listener ??= await SmsAutofill.addListener('codeReceived', ({ code }) => {
        const normalizedCode = code.replace(/\D/g, '').slice(0, 4);
        if (normalizedCode.length === 4) {
          this.codeSubject.next(normalizedCode);
        }
      });

      await SmsAutofill.startListening();
    } catch {
      // Manual code entry remains available if Google Play services is unavailable.
    }
  }
}
