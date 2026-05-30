import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user-model';
import { BACKEND_URL } from '../globals';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  constructor(private http: HttpClient) { }

  getProfile(token: string): Observable<UserModel> {
    return this.http.get<UserModel>(
      `${BACKEND_URL}/api/profile`,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
  }
}
