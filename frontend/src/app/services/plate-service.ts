import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Plate } from '../models/plate-model';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PlateService {
  private configService = inject(ConfigService);

  constructor(private http: HttpClient) { }

  getPlates(): Observable<Plate[]> {
    return this.http.get<Plate[]>(`${this.configService.backendUrl}/api/Plates`);
  }

  createPlate(number: string, checkerGmail: string): Observable<Plate> {
    return this.http.post<Plate>(`${this.configService.backendUrl}/api/Plates`, { number, checkerGmail });
  }

  updatePlate(id: number, number: string, checkerGmail: string): Observable<void> {
    return this.http.put<void>(`${this.configService.backendUrl}/api/Plates/${id}`, { number, checkerGmail });
  }

  deletePlate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/Plates/${id}`);
  }
}
