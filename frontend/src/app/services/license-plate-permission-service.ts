import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LicensePlatePermissionModel } from '../models/license-plate-permission-model';
import { BACKEND_URL } from '../globals';

@Injectable({
  providedIn: 'root',
})
export class LicensePlatePermissionService {
  constructor(private http: HttpClient) { }

  getPlatePermission(
    plate: string
  ): Observable<LicensePlatePermissionModel> {
    return this.http.get<LicensePlatePermissionModel>(
      `${BACKEND_URL}/api/permissions/${encodeURIComponent(plate)}`
    );
    // No catchError here — let 404 reach the subscriber's error callback
    // so the component can distinguish "plate not found" from "plate found, no cap"
  }
}