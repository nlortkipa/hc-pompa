import { Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChildren, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth-service';

@Component({
  standalone: true,
  selector: 'app-verify',
  imports: [RouterLink, CommonModule],
  templateUrl: './verify.html',
  styleUrl: './verify.scss',
})
export class Verify implements OnInit, OnDestroy {
  @ViewChildren('otpInput')
  otpInputs!: QueryList<ElementRef<HTMLInputElement>>;

  constructor(private router: Router, public authService: AuthService) { }

  timeLeft = signal('');
  error = signal('');
  showError = signal(false);
  private timerInterval?: ReturnType<typeof setInterval>;

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
  }

  ngOnDestroy() {
    clearInterval(this.timerInterval);
  }

  private updateTimeLeft(expiresAt: Date): number {
    const secondsLeft = Math.max(0, Math.floor((new Date(expiresAt).getTime() - Date.now()) / 1000));
    const m = Math.floor(secondsLeft / 60).toString().padStart(2, '0');
    const s = (secondsLeft % 60).toString().padStart(2, '0');
    this.timeLeft.set(`${m}:${s}`);
    return secondsLeft;
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

    this.authService.verifyCode(otp).subscribe({
      next: (token) => {
        this.authService.setCode(null);
        localStorage.setItem('token', token)
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error.set(err.error);
        localStorage.removeItem('token')
        this.showError.set(true);
      }
    });
  }

  onOtpInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/\D/g, '');
    this.showError.set(false);
    if (input.value && index < this.otpInputs.length - 1) {
      this.otpInputs.get(index + 1)?.nativeElement.focus();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;
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
    this.showError.set(false);
    pasted.split('').forEach((char, i) => {
      const input = this.otpInputs.get(i)?.nativeElement;
      if (input) input.value = char;
    });
    this.otpInputs.get(pasted.length - 1)?.nativeElement.focus();
  }
}