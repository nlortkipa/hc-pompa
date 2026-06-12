import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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

  getAll(filter?: any): Observable<PumpOperationModel[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
      if (filter.toDate) params = params.set('toDate', filter.toDate);
      if (filter.companyId) params = params.set('companyId', filter.companyId.toString());
      if (filter.addressId) params = params.set('addressId', filter.addressId.toString());
      if (filter.operatorId) params = params.set('operatorId', filter.operatorId.toString());
    }
    return this.http.get<PumpOperationModel[]>(`${BACKEND_URL}/api/pumpoperations`, { params });
  }
}
