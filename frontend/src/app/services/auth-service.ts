import { Injectable, signal } from '@angular/core';
import { BACKEND_URL } from '../globals';
import { HttpClient } from '@angular/common/http';
import { SmsCodeModel } from '../models/sms-code-model';
import { tap } from 'rxjs';

const SMS_CODE_KEY = 'sms_code';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  smsCode = signal<SmsCodeModel | null>(this.loadCode());

  constructor(private http: HttpClient) { }

  sendCode(phone: string) {
    return this.http.post<SmsCodeModel>(
      `${BACKEND_URL}/api/auth/send-code`,
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
}