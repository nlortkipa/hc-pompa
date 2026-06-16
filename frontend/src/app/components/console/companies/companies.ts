import { Component, OnInit, signal, ChangeDetectorRef, effect } from '@angular/core';
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
  filteredCompanies = signal<CompanyModel[]>([]);
  
  addresses = signal<AddressModel[]>([]);
  filteredAddresses = signal<AddressModel[]>([]);

  isLoading = false;
  
  selectedCompanyId: number | null = null;
  companyForm = { id: 0, name: '' };
  addressForm = { id: 0, name: '' };
  
  isEditingCompany = false;
  isEditingAddress = false;

  constructor(
    public companyService: CompanyService,
    private cdr: ChangeDetectorRef
  ) {
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
    this.addressForm.name = ''; // Reset address input on switch
    this.companyService.getAddresses(id);
    this.cancelAddressEdit();
  }

  // Company Actions
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
          alert(err.error || 'კომპანია მსგავსი სახელით უკვე არსებობს.');
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

  // Address Actions
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

    // Check for duplicate address name
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
