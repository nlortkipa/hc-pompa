import { Injectable, signal, inject } from '@angular/core';
import { CompanyModel } from '../models/company-model';
import { AddressModel } from '../models/address-model';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable, Subscription } from 'rxjs';

export interface CompanyExcelImportResult {
  totalRows: number;
  uniquePairs: number;
  duplicateRowsSkipped: number;
  createdCompanies: number;
  existingCompanies: number;
  createdAddresses: number;
  existingAddresses: number;
  createdLinks: number;
  existingLinks: number;
}

@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  companies = signal<CompanyModel[]>([]);
  addresses = signal<AddressModel[]>([]);
  private configService = inject(ConfigService);
  private companiesRequest?: Subscription;
  private addressesRequest?: Subscription;

  constructor(private http: HttpClient) {}

  /**
   * (Re)loads the company list. Pages that show companies call this on init, so a
   * failed or stale load (e.g. before sign-in, or a backend cold start) never sticks.
   * On error the previous list is kept rather than wiped.
   */
  getCompanies() {
    this.companiesRequest?.unsubscribe();
    this.companiesRequest = this.http.get<CompanyModel[]>(`${this.configService.backendUrl}/api/companies`).subscribe({
      next: (data) => this.companies.set(data),
    });
  }

  createCompany(name: string): Observable<CompanyModel> {
    return this.http.post<CompanyModel>(`${this.configService.backendUrl}/api/companies`, { name });
  }

  updateCompany(id: number, name: string): Observable<void> {
    return this.http.put<void>(`${this.configService.backendUrl}/api/companies/${id}`, { name });
  }

  deleteCompany(id: number): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/companies/${id}`);
  }

  getAddresses(companyId: number) {
    // Cancel the previous company's request so a slow response can't overwrite this one.
    this.addressesRequest?.unsubscribe();
    this.addresses.set([]);
    this.addressesRequest = this.http
      .get<AddressModel[]>(`${this.configService.backendUrl}/api/companies/${companyId}/addresses`)
      .subscribe({
        next: (data) => this.addresses.set(data),
        error: () => this.addresses.set([]),
      });
  }

  addAddress(companyId: number, name: string): Observable<AddressModel> {
    return this.http.post<AddressModel>(`${this.configService.backendUrl}/api/companies/${companyId}/addresses`, { name });
  }

  updateAddress(addressId: number, name: string): Observable<void> {
    return this.http.put<void>(`${this.configService.backendUrl}/api/companies/addresses/${addressId}`, { name });
  }

  deleteAddress(companyId: number, addressId: number): Observable<void> {
    return this.http.delete<void>(`${this.configService.backendUrl}/api/companies/${companyId}/addresses/${addressId}`);
  }

  importExcel(file: File): Observable<CompanyExcelImportResult> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<CompanyExcelImportResult>(
      `${this.configService.backendUrl}/api/companies/import-excel`,
      formData
    );
  }
}
