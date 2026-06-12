import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BACKEND_URL } from '../globals';
import { Observable } from 'rxjs';
import { PumpOperationModel } from '../models/pump-operation';

@Injectable({
  providedIn: 'root',
})
export class PumpOperationService {
  constructor(private http: HttpClient) {}

  create(operation: PumpOperationModel): Observable<any> {
    return this.http.post(`${BACKEND_URL}/api/pumpoperations`, operation);
  }

  getAll(): Observable<PumpOperationModel[]> {
    return this.http.get<PumpOperationModel[]>(`${BACKEND_URL}/api/pumpoperations`);
  }
}
