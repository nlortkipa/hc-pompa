import { Injectable, signal, inject } from '@angular/core';
import { CompanyModel } from '../models/company-model';
import { AddressModel } from '../models/address-model';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  companies = signal<CompanyModel[]>([]);
  addresses = signal<AddressModel[]>([]);
  private configService = inject(ConfigService);

  constructor(private http: HttpClient) {
    this.getCompanies();
  }

  getCompanies() {
    this.http.get<CompanyModel[]>(`${this.configService.backendUrl}/api/companies`).subscribe({
      next: (data) => this.companies.set(data),
      error: () => this.companies.set([]),
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
    this.addresses.set([]);
    this.http
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
}