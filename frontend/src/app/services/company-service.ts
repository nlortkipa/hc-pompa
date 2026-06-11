import { Injectable, signal } from '@angular/core';
import { CompanyModel } from '../models/company-model';
import { AddressModel } from '../models/address-model';
import { LicensePlatePermissionModel } from '../models/license-plate-permission-model';
import { HttpClient } from '@angular/common/http';
import { BACKEND_URL } from '../globals';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  companies = signal<CompanyModel[]>([]);
  addresses = signal<AddressModel[]>([]);

  constructor(private http: HttpClient) {
    this.getCompanies();
  }

  private getCompanies() {
    this.http.get<CompanyModel[]>(`${BACKEND_URL}/api/companies`).subscribe({
      next: (data) => this.companies.set(data),
      error: () => this.companies.set([]),
    });
  }

  getAddresses(companyId: number) {
    this.addresses.set([]);
    this.http
      .get<AddressModel[]>(`${BACKEND_URL}/api/companies/${companyId}/addresses`)
      .subscribe({
        next: (data) => this.addresses.set(data),
        error: () => this.addresses.set([]),
      });
  }
}