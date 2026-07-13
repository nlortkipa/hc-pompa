import { Injectable, signal } from '@angular/core';
import { ConfigService } from './config.service';
import { HttpClient } from '@angular/common/http';
import { SmsCodeModel } from '../models/sms-code-model';
import { tap, throwError } from 'rxjs';

const SMS_CODE_KEY = 'sms_code';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  smsCode = signal<SmsCodeModel | null>(this.loadCode());

  constructor(private http: HttpClient, private configService: ConfigService) { }

  sendCode(phone: string) {
    return this.http.post<SmsCodeModel>(
      `${this.configService.backendUrl}/api/auth/send-code`,
      JSON.stringify(phone),
      { headers: { 'Content-Type': 'application/json' } }
    ).pipe(
      tap(code => this.setCode(code))
    );
  }

  setCode(code: SmsCodeModel | null) {
    this.smsCode.set(code);
    if (code) localStorage.setItem(SMS_CODE_KEY, JSON.stringify(code));
    else localStorage.removeItem(SMS_CODE_KEY);
  }

  private loadCode(): SmsCodeModel | null {
    try {
      const raw = localStorage.getItem(SMS_CODE_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }

  verifyCode(code: string) {
    const smsCode = this.smsCode();
    if (!smsCode) return throwError(() => new Error('აქტიური სმს კოდი არ არის'));

    return this.http.post(
      `${this.configService.backendUrl}/api/auth/verify-code`,
      { userId: smsCode.userId, code },
      { responseType: 'text' }
    );
  }
}
