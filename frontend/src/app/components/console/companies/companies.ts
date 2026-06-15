import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../../../services/company-service';
import { CompanyModel } from '../../../models/company-model';
import { AddressModel } from '../../../models/address-model';
import { finalize } from 'rxjs/operators';
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
  addresses = signal<AddressModel[]>([]);
  isLoading = false;
  
  selectedCompanyId: number | null = null;
  companyForm = { id: 0, name: '' };
  addressForm = { id: 0, name: '' };
  
  isEditingCompany = false;
  isEditingAddress = false;

  constructor(
    public companyService: CompanyService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.isLoading = true;
    this.companyService.getCompanies();
    // Signal based service, but we want to know when it's done for loading state
    // Let's just use it as is for now as it's already integrated
    this.isLoading = false;
    this.cdr.detectChanges();
  }

  selectCompany(id: number): void {
    this.selectedCompanyId = id;
    this.companyService.getAddresses(id);
    this.cancelAddressEdit();
  }

  // Company Actions
  editCompany(company: CompanyModel): void {
    this.isEditingCompany = true;
    this.companyForm = { ...company };
  }

  cancelCompanyEdit(): void {
    this.isEditingCompany = false;
    this.companyForm = { id: 0, name: '' };
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

  // Address Actions
  editAddress(address: AddressModel): void {
    this.isEditingAddress = true;
    this.addressForm = { ...address };
  }

  cancelAddressEdit(): void {
    this.isEditingAddress = false;
    this.addressForm = { id: 0, name: '' };
  }

  saveAddress(): void {
    if (!this.selectedCompanyId || !this.addressForm.name.trim()) return;
    this.isLoading = true;

    const obs: Observable<any> = this.isEditingAddress
      ? this.companyService.updateAddress(this.addressForm.id, this.addressForm.name)
      : this.companyService.addAddress(this.selectedCompanyId, this.addressForm.name);

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
