import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BACKEND_URL } from '../globals';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user-model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<UserModel[]> {
    return this.http.get<UserModel[]>(`${BACKEND_URL}/api/users`);
  }

  getById(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${BACKEND_URL}/api/users/${id}`);
  }

  create(user: any): Observable<UserModel> {
    return this.http.post<UserModel>(`${BACKEND_URL}/api/users`, user);
  }

  update(id: number, user: any): Observable<UserModel> {
    return this.http.put<UserModel>(`${BACKEND_URL}/api/users/${id}`, user);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${BACKEND_URL}/api/users/${id}`);
  }
}
