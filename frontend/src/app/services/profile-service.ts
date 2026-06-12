import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { UserModel } from '../models/user-model';
import { BACKEND_URL } from '../globals';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  profile = signal<UserModel | null>(null);

  constructor(private http: HttpClient) { }

  getProfile(token: string): Observable<UserModel> {
    return this.http.get<UserModel>(
      `${BACKEND_URL}/api/profile`,
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
}
