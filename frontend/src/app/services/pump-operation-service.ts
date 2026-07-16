import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs';
import { PumpOperationFilter, PumpOperationModel, PumpOperationReviewModel } from '../models/pump-operation';

@Injectable({
  providedIn: 'root',
})
export class PumpOperationService {
  private configService = inject(ConfigService);

  constructor(private http: HttpClient) {}

  create(operation: PumpOperationModel): Observable<any> {
    return this.http.post(`${this.configService.backendUrl}/api/pumpoperations`, operation);
  }

  getAll(filter?: PumpOperationFilter): Observable<PumpOperationModel[]> {
    const params = this.buildParams(filter);

    return this.http.get<PumpOperationModel[]>(`${this.configService.backendUrl}/api/pumpoperations`, { params });
  }

  downloadExcel(filter?: PumpOperationFilter): Observable<Blob> {
    const params = this.buildParams(filter);

    return this.http.get(`${this.configService.backendUrl}/api/pumpoperations/export`, {
      params,
      responseType: 'blob',
    });
  }

  private buildParams(filter?: PumpOperationFilter): HttpParams {
    let params = new HttpParams();
    if (filter) {
      if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
      if (filter.toDate) params = params.set('toDate', filter.toDate);
      if (filter.companyId != null) params = params.set('companyId', filter.companyId.toString());
      if (filter.companyName) params = params.set('companyName', filter.companyName);
      if (filter.addressId != null) params = params.set('addressId', filter.addressId.toString());
      if (filter.operatorId != null) params = params.set('operatorId', filter.operatorId.toString());
      if (filter.operatorName) params = params.set('operatorName', filter.operatorName);
      if (filter.plateNumber) params = params.set('plateNumber', filter.plateNumber);
    }

    return params;
  }

  getReview(token: string): Observable<PumpOperationReviewModel> {
    return this.http.get<PumpOperationReviewModel>(`${this.configService.backendUrl}/api/pumpoperations/review/${token}`);
  }

  acceptReview(token: string): Observable<PumpOperationReviewModel> {
    return this.http.post<PumpOperationReviewModel>(`${this.configService.backendUrl}/api/pumpoperations/review/${token}/accept`, {});
  }

  rejectReview(token: string): Observable<PumpOperationReviewModel> {
    return this.http.post<PumpOperationReviewModel>(`${this.configService.backendUrl}/api/pumpoperations/review/${token}/reject`, {});
  }
}
