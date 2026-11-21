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

  createPlate(number: string, type: string, isStationary: boolean, checkerUserIds: number[]): Observable<Plate> {
    return this.http.post<Plate>(`${this.configService.backendUrl}/api/Plates`, { number, type, isStationary, checkerUserIds });
  }

  /** The result says how many reports the change was carried over to. */
  updatePlate(id: number, number: string, type: string, isStationary: boolean, checkerUserIds: number[]): Observable<PlateUpdateResult> {
    return this.http.put<PlateUpdateResult>(`${this.configService.backendUrl}/api/Plates/${id}`, { number, type, isStationary, checkerUserIds });
  }

  deletePlate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/Plates/${id}`);
  }
}

export interface PlateUpdateResult {
  /** Reports (any status) whose plate number was renamed. */
  renamedReports: number;
  /** Pending reports whose pump type was updated. */
  retypedReports: number;
  /** Pending review requests removed because their checker left the plate. */
  removedReviewRequests: number;
  /** Pending review requests created for newly assigned checkers. */
  addedReviewRequestCount: number;
}
