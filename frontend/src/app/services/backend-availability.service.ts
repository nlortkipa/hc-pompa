import { DestroyRef, Injectable, signal } from '@angular/core';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class BackendAvailabilityService {
  readonly isUnavailable = signal(false);
  readonly isChecking = signal(false);

  private pollTimer?: ReturnType<typeof setInterval>;

  constructor(
    private configService: ConfigService,
    destroyRef: DestroyRef
  ) {
    destroyRef.onDestroy(() => this.stopPolling());
  }

  markUnavailable(): void {
    if (!this.isUnavailable()) {
      this.isUnavailable.set(true);
      this.startPolling();
    }
  }

  markAvailable(): void {
    this.isUnavailable.set(false);
    this.stopPolling();
  }

  async retry(): Promise<void> {
    if (this.isChecking()) return;

    this.isChecking.set(true);
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 8000);

    try {
      const backendUrl = this.configService.backendUrl.replace(/\/$/, '');
      const response = await fetch(`${backendUrl}/api/companies`, {
        method: 'GET',
        cache: 'no-store',
        signal: controller.signal,
        headers: this.authorizationHeaders()
      });

      if ([502, 503, 504].includes(response.status)) {
        throw new Error('Backend unavailable');
      }

      this.markAvailable();
    } catch {
      this.isUnavailable.set(true);
      this.startPolling();
    } finally {
      clearTimeout(timeoutId);
      this.isChecking.set(false);
    }
  }

  private startPolling(): void {
    if (this.pollTimer) return;

    this.pollTimer = setInterval(() => {
      void this.retry();
    }, 5000);
  }

  private stopPolling(): void {
    if (!this.pollTimer) return;

    clearInterval(this.pollTimer);
    this.pollTimer = undefined;
  }

  private authorizationHeaders(): HeadersInit {
    const token = localStorage.getItem('token');
    return token ? { Authorization: `Bearer ${token}` } : {};
  }
}
