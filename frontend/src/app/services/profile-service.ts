import { HttpClient } from '@angular/common/http';
import { Injectable, signal, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { UserModel } from '../models/user-model';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  profile = signal<UserModel | null>(null);
  private configService = inject(ConfigService);

  constructor(private http: HttpClient) { }

  getProfile(token: string): Observable<UserModel> {
    return this.http.get<UserModel>(
      `${this.configService.backendUrl}/api/profile`,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    ).pipe(
      tap(u => this.profile.set(u))
    );
  }

  getProfileData(): UserModel | null {
    return this.profile();
  }

  updateAccount(account: { username: string; avatarUrl: string | null }): Observable<UserModel> {
    return this.http.put<UserModel>(
      `${this.configService.backendUrl}/api/profile/account`,
      account
    ).pipe(
      tap(u => this.profile.set(u))
    );
  }

  logoutAllDevices(): Observable<void> {
    return this.http.post<void>(
      `${this.configService.backendUrl}/api/profile/logout-all-devices`,
      {}
    );
  }

  deleteAccount(): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/profile`);
  }

  clearProfile(): void {
    this.profile.set(null);
  }
}
