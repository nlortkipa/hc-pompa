import { Component, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { APP_NAME } from '../../globals';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth-service';
import { finalize } from 'rxjs';
import { Capacitor } from '@capacitor/core';
import { SmsAutofillService } from '../../services/sms-autofill.service';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  appName = APP_NAME;
  readonly isNativeApp = Capacitor.isNativePlatform();

  number = signal('');
  error = signal('');
  showError = signal(false);
  isLoading = signal(false);

  constructor(
    private router: Router,
    private authService: AuthService,
    private smsAutofill: SmsAutofillService,
  ) { }

  ngOnInit() {
    const token = localStorage.getItem('token')
    if (token != null)
      this.router.navigate(['/console'])
  }

  async submit() {
    if (this.isLoading()) return;

    if (!this.number().trim()) {
      this.error.set('შეიყვანეთ ტელეფონის ნომერი');
      this.showError.set(true);
      return;
    }

    const cleanNumber = this.number().replace(/\D/g, '');
    if (!/^\d{9}$/.test(cleanNumber)) {
      this.error.set('შეიყვანეთ სწორი 9-ნიშნა ტელეფონის ნომერი');
      this.showError.set(true);
      return;
    }

    this.isLoading.set(true);
    this.showError.set(false);
    this.error.set('');

    // Android must begin listening before the backend sends the SMS.
    await this.smsAutofill.startListening();

    this.authService.sendCode(cleanNumber)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: () => {
          this.router.navigate(['/verify']);
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(this.getSendCodeError(err));
          this.showError.set(true);
        }
      });
  }

  private getSendCodeError(err: HttpErrorResponse): string {
    const apiMessage = typeof err.error === 'string' ? err.error.trim() : '';

    if (err.status === 404) {
      return apiMessage || 'ნომერი ვერ მოიძებნა';
    }

    if (err.status === 400) {
      return apiMessage || 'ტელეფონის ნომერი არასწორია';
    }

    if (err.status === 429) {
      return 'ძალიან ბევრი მოთხოვნა გაიგზავნა. სცადეთ ცოტა ხანში';
    }

    if (err.status === 0) {
      return 'სერვერთან დაკავშირება ვერ მოხერხდა. შეამოწმეთ ინტერნეტი';
    }

    if (err.status >= 500) {
      return 'სმს კოდის გაგზავნა დროებით ვერ ხერხდება. სცადეთ მოგვიანებით';
    }

    return 'მოთხოვნის დამუშავება ვერ მოხერხდა. სცადეთ თავიდან';
  }

}
