import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user-model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private configService = inject(ConfigService);

  constructor(private http: HttpClient) {}

  getAll(): Observable<UserModel[]> {
    return this.http.get<UserModel[]>(`${this.configService.backendUrl}/api/users`);
  }

  getById(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.configService.backendUrl}/api/users/${id}`);
  }

  create(user: any): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.configService.backendUrl}/api/users`, user);
  }

  update(id: number, user: any): Observable<UserModel> {
    return this.http.put<UserModel>(`${this.configService.backendUrl}/api/users/${id}`, user);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/users/${id}`);
  }
}
