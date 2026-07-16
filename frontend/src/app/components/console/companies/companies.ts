import { Component, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyExcelImportResult, CompanyService } from '../../../services/company-service';
import { CompanyModel } from '../../../models/company-model';
import { AddressModel } from '../../../models/address-model';
import { finalize, timeout } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-companies',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './companies.html',
  styleUrls: ['./companies.scss'],
})
export class Companies implements OnInit {
  companies = signal<CompanyModel[]>([]);
  filteredCompanies = signal<CompanyModel[]>([]);
  
  addresses = signal<AddressModel[]>([]);
  filteredAddresses = signal<AddressModel[]>([]);

  isLoading = false;
  isImporting = signal(false);
  selectedExcelFile = signal<File | null>(null);
  importResult = signal<CompanyExcelImportResult | null>(null);
  importError = signal('');
  
  selectedCompanyId: number | null = null;
  companyForm = { id: 0, name: '' };
  addressForm = { id: 0, name: '' };
  
  isEditingCompany = false;
  isEditingAddress = false;

  constructor(public companyService: CompanyService) {
    effect(() => {
        this.companies.set(this.companyService.companies());
        this.filterCompanies();
    });
    effect(() => {
        this.addresses.set(this.companyService.addresses());
        this.filterAddresses();
    });
  }

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.companyService.getCompanies();
  }

  onExcelSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedExcelFile.set(input.files?.[0] ?? null);
    this.importResult.set(null);
    this.importError.set('');
  }

  importExcel(): void {
    const file = this.selectedExcelFile();
    if (!file || this.isImporting()) return;
    if (!confirm('Excel ფაილი მთლიანად ჩაანაცვლებს კომპანიებისა და მისამართების მიმდინარე კატალოგს. გაგრძელება?')) return;

    this.isImporting.set(true);
    this.importResult.set(null);
    this.importError.set('');

    this.companyService.importExcel(file)
      .pipe(
        timeout(60_000),
        finalize(() => this.isImporting.set(false))
      )
      .subscribe({
        next: (result) => {
          this.importResult.set(result);
          this.companyService.getCompanies();
          if (this.selectedCompanyId) {
            this.companyService.getAddresses(this.selectedCompanyId);
          }
        },
        error: () => this.importError.set('არასწორი ფორმატი')
      });
  }

  filterCompanies(): void {
    const term = this.companyForm.name.toLowerCase();
    this.filteredCompanies.set(
      this.companies().filter((c) => c.name.toLowerCase().includes(term))
    );
  }

  filterAddresses(): void {
    const term = this.addressForm.name.toLowerCase();
    this.filteredAddresses.set(
      this.addresses().filter((a) => a.name.toLowerCase().includes(term))
    );
  }

  selectCompany(id: number): void {
    this.selectedCompanyId = id;
    this.addressForm.name = '';
    this.companyService.getAddresses(id);
    this.cancelAddressEdit();
  }

  editCompany(company: CompanyModel): void {
    this.isEditingCompany = true;
    this.companyForm = { ...company };
    this.filterCompanies();
  }

  cancelCompanyEdit(): void {
    this.isEditingCompany = false;
    this.companyForm = { id: 0, name: '' };
    this.filterCompanies();
  }

  saveCompany(): void {
    if (!this.companyForm.name.trim()) return;
    this.isLoading = true;

    const obs: Observable<any> = this.isEditingCompany
      ? this.companyService.updateCompany(this.companyForm.id, this.companyForm.name)
      : this.companyService.createCompany(this.companyForm.name);

    obs.pipe(finalize(() => (this.isLoading = false))).subscribe({
      next: () => {
        this.companyService.getCompanies();
        this.cancelCompanyEdit();
      },
      error: (err) => {
        if (err.status === 409) {
          alert('კომპანია მსგავსი სახელით უკვე არსებობს.');
        } else {
          alert('შენახვა ვერ მოხერხდა.');
        }
      }
    });
  }

  deleteCompany(id: number): void {
    if (!confirm('ნამდვილად გსურთ კომპანიის წაშლა? წაიშლება ყველა დაკავშირებული მისამართიც.')) return;
    this.isLoading = true;
    this.companyService.deleteCompany(id).pipe(finalize(() => (this.isLoading = false))).subscribe({
      next: () => {
        this.companyService.getCompanies();
        if (this.selectedCompanyId === id) this.selectedCompanyId = null;
      }
    });
  }

  editAddress(address: AddressModel): void {
    this.isEditingAddress = true;
    this.addressForm = { ...address };
    this.filterAddresses();
  }

  cancelAddressEdit(): void {
    this.isEditingAddress = false;
    this.addressForm = { id: 0, name: '' };
    this.filterAddresses();
  }

  saveAddress(): void {
    if (!this.selectedCompanyId || !this.addressForm.name.trim()) return;

    const addressName = this.addressForm.name.trim();
    const isDuplicate = this.companyService.addresses().some(
      (a) => a.name.toLowerCase() === addressName.toLowerCase() && 
             (!this.isEditingAddress || a.id !== this.addressForm.id)
    );

    if (isDuplicate) {
      alert('მითითებული მისამართი უკვე არსებობს ამ კომპანიისთვის.');
      return;
    }

    this.isLoading = true;

    const obs: Observable<any> = this.isEditingAddress
      ? this.companyService.updateAddress(this.addressForm.id, addressName)
      : this.companyService.addAddress(this.selectedCompanyId, addressName);

    obs.pipe(finalize(() => (this.isLoading = false))).subscribe({
      next: () => {
        this.companyService.getAddresses(this.selectedCompanyId!);
        this.cancelAddressEdit();
      }
    });
  }

  deleteAddress(addressId: number): void {
    if (!this.selectedCompanyId || !confirm('ნამდვილად გსურთ მისამართის წაშლა?')) return;
    this.isLoading = true;
    this.companyService.deleteAddress(this.selectedCompanyId, addressId).pipe(finalize(() => (this.isLoading = false))).subscribe({
      next: () => {
        this.companyService.getAddresses(this.selectedCompanyId!);
      }
    });
  }
}
