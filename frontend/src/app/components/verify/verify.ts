import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChildren, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth-service';
import { SmsAutofillService } from '../../services/sms-autofill.service';
import { Subscription } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-verify',
  imports: [RouterLink, CommonModule],
  templateUrl: './verify.html',
  styleUrl: './verify.scss',
})
export class Verify implements OnInit, AfterViewInit, OnDestroy {
  @ViewChildren('otpInput')
  otpInputs!: QueryList<ElementRef<HTMLInputElement>>;

  constructor(
    private router: Router,
    public authService: AuthService,
    private smsAutofill: SmsAutofillService,
  ) { }

  error = signal('');
  showError = signal(false);
  isLoading = signal(false);
  private timerInterval?: ReturnType<typeof setInterval>;
  private autofillSubscription?: Subscription;
  private pendingAutofillCode?: string;

  ngOnInit() {
    const code = this.authService.smsCode();

    if (!code) {
      this.router.navigate(['/login']);
      return;
    }

    const secondsLeft = this.updateTimeLeft(code.expiresAt);
    if (secondsLeft <= 0) { this.expireCode(); return; }

    this.timerInterval = setInterval(() => {
      const secondsLeft = this.updateTimeLeft(code.expiresAt);
      if (secondsLeft <= 0) this.expireCode();
    }, 1000);

    this.autofillSubscription = this.smsAutofill.code$.subscribe(code => {
      if (code) this.fillOtp(code);
    });
  }

  ngAfterViewInit() {
    if (this.pendingAutofillCode) {
      this.fillOtp(this.pendingAutofillCode);
    }
  }

  ngOnDestroy() {
    clearInterval(this.timerInterval);
    this.autofillSubscription?.unsubscribe();
  }

  private updateTimeLeft(expiresAt: Date): number {
    return Math.max(0, Math.floor((new Date(expiresAt).getTime() - Date.now()) / 1000));
  }

  private expireCode() {
    clearInterval(this.timerInterval);
    this.authService.setCode(null);
    this.router.navigate(['/login']);
  }

  submit() {
    const otp = this.otpInputs.map(i => i.nativeElement.value).join('');
    if (otp.length < this.otpInputs.length) {
      this.error.set('შეიყვანეთ სრული კოდი');
      this.showError.set(true);
      return;
    }

    this.isLoading.set(true);

    this.authService.verifyCode(otp).subscribe({
      next: (token) => {
        this.isLoading.set(false);
        this.authService.setCode(null);
        localStorage.setItem('token', token)
        this.router.navigate(['/console']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.error.set('კოდის დადასტურება ვერ მოხერხდა.');
        localStorage.removeItem('token')
        this.showError.set(true);
      }
    });
  }

  onOtpInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    const digits = input.value.replace(/\D/g, '');
    if (digits.length > 1) {
      this.fillOtp(digits);
      return;
    }

    input.value = digits;
    this.showError.set(false);
    if (input.value && index < this.otpInputs.length - 1) {
      this.otpInputs.get(index + 1)?.nativeElement.focus();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;
    if (event.key === 'Enter') {
      event.preventDefault();
      if (!this.isLoading()) this.submit();
      return;
    }

    if (event.key === 'Backspace' && !input.value && index > 0) {
      this.otpInputs.get(index - 1)?.nativeElement.focus();
    }
  }

  onPaste(event: ClipboardEvent) {
    event.preventDefault();
    const pasted = event.clipboardData
      ?.getData('text')
      .replace(/\D/g, '')
      .slice(0, this.otpInputs.length);
    if (!pasted) return;
    this.fillOtp(pasted);
  }

  private fillOtp(value: string) {
    const code = value.replace(/\D/g, '').slice(0, 4);
    if (!code || !this.otpInputs || this.otpInputs.length === 0) {
      this.pendingAutofillCode = code;
      return;
    }

    this.pendingAutofillCode = undefined;
    this.showError.set(false);
    code.split('').forEach((char, i) => {
      const input = this.otpInputs.get(i)?.nativeElement;
      if (input) input.value = char;
    });
    this.otpInputs.get(Math.min(code.length, this.otpInputs.length) - 1)?.nativeElement.focus();
  }
}
