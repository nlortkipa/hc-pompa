import { Injectable, signal } from '@angular/core';
import { CompanyModel } from '../models/company-model';
import { HttpClient } from '@angular/common/http';
import { BACKEND_URL } from '../globals';

@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  companies = signal<CompanyModel[]>([])

  constructor(private http: HttpClient) {
    this.getCompanies()
  }

  private getCompanies() {
    this.http.get<CompanyModel[]>(`${BACKEND_URL}/api/companies`).subscribe({
      next: (data) => this.companies.set(data),
      error: () => this.companies.set([])
    })
  }
}
