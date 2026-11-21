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

  getById(id: number): Observable<PumpOperationModel> {
    return this.http.get<PumpOperationModel>(`${this.configService.backendUrl}/api/pumpoperations/${id}`);
  }

  /** Only the creator's own pending operations can be updated; checkers get fresh review links. */
  update(id: number, operation: PumpOperationModel): Observable<PumpOperationModel> {
    return this.http.put<PumpOperationModel>(`${this.configService.backendUrl}/api/pumpoperations/${id}`, operation);
  }

  /** Only the creator's own pending operations can be deleted. */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/pumpoperations/${id}`);
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

  /** Review requests assigned to the signed-in user, pending first. */
  getMyReviews(): Observable<PumpOperationReviewModel[]> {
    return this.http.get<PumpOperationReviewModel[]>(`${this.configService.backendUrl}/api/pumpoperations/reviews/mine`);
  }

  getReview(token: string): Observable<PumpOperationReviewModel> {
    return this.http.get<PumpOperationReviewModel>(`${this.configService.backendUrl}/api/pumpoperations/review/${token}`);
  }

  /**
   * `version` is the one the checker saw; if the operation was edited since, the
   * server answers 409 with the current data instead of recording the decision.
   */
  acceptReview(token: string, version?: string): Observable<PumpOperationReviewModel> {
    return this.http.post<PumpOperationReviewModel>(`${this.configService.backendUrl}/api/pumpoperations/review/${token}/accept`, {}, { params: this.versionParams(version) });
  }

  rejectReview(token: string, version?: string): Observable<PumpOperationReviewModel> {
    return this.http.post<PumpOperationReviewModel>(`${this.configService.backendUrl}/api/pumpoperations/review/${token}/reject`, {}, { params: this.versionParams(version) });
  }

  private versionParams(version?: string): HttpParams {
    return version ? new HttpParams().set('version', version) : new HttpParams();
  }
}
