import { Component, HostListener, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyExcelImportResult, CompanyService } from '../../../services/company-service';
import { CompanyModel } from '../../../models/company-model';
import { AddressModel } from '../../../models/address-model';
import { finalize, timeout } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Icon } from '../../icon/icon';
import { UiService } from '../../../services/ui.service';

interface EditSheet {
  kind: 'company' | 'address';
  id: number;
  name: string;
}

@Component({
  selector: 'app-companies',
  standalone: true,
  imports: [CommonModule, FormsModule, Icon],
  templateUrl: './companies.html',
  styleUrls: ['./companies.scss'],
})
export class Companies implements OnInit {
  companyService = inject(CompanyService);
  private ui = inject(UiService);

  companySearch = signal('');
  addressSearch = signal('');
  selectedCompanyId = signal<number | null>(null);

  filteredCompanies = computed(() => {
    const term = this.companySearch().trim().toLowerCase();
    return this.companyService.companies().filter(c => c.name.toLowerCase().includes(term));
  });

  filteredAddresses = computed(() => {
    const term = this.addressSearch().trim().toLowerCase();
    return this.companyService.addresses().filter(a => a.name.toLowerCase().includes(term));
  });

  selectedCompany = computed(() =>
    this.companyService.companies().find(c => c.id === this.selectedCompanyId()) ?? null
  );

  isSaving = false;
  sheet: EditSheet | null = null;
  sheetError = '';

  showImport = false;
  isImporting = signal(false);
  selectedExcelFile = signal<File | null>(null);
  importResult = signal<CompanyExcelImportResult | null>(null);
  importError = signal('');
  isDragging = false;

  ngOnInit(): void {
    this.companyService.getCompanies();
    this.companyService.addresses.set([]);
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.ui.dialog()) return;
    if (this.sheet) this.closeSheet();
    else if (this.showImport) this.closeImport();
  }

  // ---------- Selection ----------

  selectCompany(id: number): void {
    if (this.selectedCompanyId() === id) return;
    this.selectedCompanyId.set(id);
    this.addressSearch.set('');
    this.companyService.addresses.set([]);
    this.companyService.getAddresses(id);
  }

  clearSelection(): void {
    this.selectedCompanyId.set(null);
  }

  // ---------- Create / edit sheet ----------

  newCompany(): void {
    this.openSheet({ kind: 'company', id: 0, name: this.companySearch().trim() });
  }

  editCompany(company: CompanyModel): void {
    this.openSheet({ kind: 'company', id: company.id, name: company.name });
  }

  newAddress(): void {
    this.openSheet({ kind: 'address', id: 0, name: this.addressSearch().trim() });
  }

  editAddress(address: AddressModel): void {
    this.openSheet({ kind: 'address', id: address.id, name: address.name });
  }

  private openSheet(sheet: EditSheet): void {
    this.sheet = sheet;
    this.sheetError = '';
  }

  closeSheet(): void {
    this.sheet = null;
    this.sheetError = '';
  }

  saveSheet(): void {
    const sheet = this.sheet;
    if (!sheet || this.isSaving) return;
    const name = sheet.name.trim();
    if (!name) {
      this.sheetError = sheet.kind === 'company' ? 'შეიყვანეთ კომპანიის სახელი' : 'შეიყვანეთ მისამართი';
      return;
    }
    if (sheet.kind === 'company') this.saveCompany(sheet, name);
    else this.saveAddress(sheet, name);
  }

  private saveCompany(sheet: EditSheet, name: string): void {
    this.isSaving = true;
    const obs: Observable<any> = sheet.id
      ? this.companyService.updateCompany(sheet.id, name)
      : this.companyService.createCompany(name);

    obs.pipe(finalize(() => (this.isSaving = false))).subscribe({
      next: () => {
        this.companyService.getCompanies();
        this.closeSheet();
        this.ui.toast(sheet.id ? 'კომპანია განახლდა' : 'კომპანია დაემატა');
      },
      error: (err) => {
        this.sheetError = err.status === 409
          ? 'კომპანია მსგავსი სახელით უკვე არსებობს.'
          : 'შენახვა ვერ მოხერხდა.';
      }
    });
  }

  private saveAddress(sheet: EditSheet, name: string): void {
    const companyId = this.selectedCompanyId();
    if (!companyId) return;

    const isDuplicate = this.companyService.addresses().some(
      a => a.name.toLowerCase() === name.toLowerCase() && a.id !== sheet.id
    );
    if (isDuplicate) {
      this.sheetError = 'მითითებული მისამართი უკვე არსებობს ამ კომპანიისთვის.';
      return;
    }

    this.isSaving = true;
    const obs: Observable<any> = sheet.id
      ? this.companyService.updateAddress(sheet.id, name)
      : this.companyService.addAddress(companyId, name);

    obs.pipe(finalize(() => (this.isSaving = false))).subscribe({
      next: () => {
        this.companyService.getAddresses(companyId);
        this.closeSheet();
        this.ui.toast(sheet.id ? 'მისამართი განახლდა' : 'მისამართი დაემატა');
      },
      error: () => (this.sheetError = 'შენახვა ვერ მოხერხდა.')
    });
  }

  // ---------- Delete ----------

  async deleteCompany(company: CompanyModel): Promise<void> {
    const ok = await this.ui.confirm({
      title: `წავშალოთ „${company.name}“?`,
      message: 'წაიშლება ყველა დაკავშირებული მისამართიც.',
      confirmLabel: 'წაშლა',
      destructive: true,
    });
    if (!ok) return;

    this.companyService.deleteCompany(company.id).subscribe({
      next: () => {
        this.companyService.getCompanies();
        if (this.selectedCompanyId() === company.id) this.clearSelection();
        this.ui.toast('კომპანია წაიშალა');
      },
      error: () => this.ui.toast('წაშლა ვერ მოხერხდა', 'error')
    });
  }

  async deleteAddress(address: AddressModel): Promise<void> {
    const companyId = this.selectedCompanyId();
    if (!companyId) return;
    const ok = await this.ui.confirm({
      title: 'წავშალოთ მისამართი?',
      message: address.name,
      confirmLabel: 'წაშლა',
      destructive: true,
    });
    if (!ok) return;

    this.companyService.deleteAddress(companyId, address.id).subscribe({
      next: () => {
        this.companyService.getAddresses(companyId);
        this.ui.toast('მისამართი წაიშალა');
      },
      error: () => this.ui.toast('წაშლა ვერ მოხერხდა', 'error')
    });
  }

  // ---------- Excel import ----------

  openImport(): void {
    this.showImport = true;
    this.importResult.set(null);
    this.importError.set('');
  }

  closeImport(): void {
    if (this.isImporting()) return;
    this.showImport = false;
    this.selectedExcelFile.set(null);
  }

  onExcelSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.setExcelFile(input.files?.[0] ?? null);
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = true;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;
    const file = event.dataTransfer?.files?.[0];
    if (file && /\.xlsx?m?$/i.test(file.name)) this.setExcelFile(file);
    else if (file) this.importError.set('აირჩიეთ .xlsx ან .xlsm ფაილი');
  }

  private setExcelFile(file: File | null): void {
    this.selectedExcelFile.set(file);
    this.importResult.set(null);
    this.importError.set('');
  }

  async importExcel(): Promise<void> {
    const file = this.selectedExcelFile();
    if (!file || this.isImporting()) return;
    const ok = await this.ui.confirm({
      title: 'ჩავანაცვლოთ კატალოგი?',
      message: 'Excel ფაილი მთლიანად ჩაანაცვლებს კომპანიებისა და მისამართების მიმდინარე კატალოგს.',
      confirmLabel: 'ჩანაცვლება',
      destructive: true,
    });
    if (!ok) return;

    this.isImporting.set(true);
    this.importResult.set(null);
    this.importError.set('');

    this.companyService.importExcel(file)
      .pipe(timeout(60_000), finalize(() => this.isImporting.set(false)))
      .subscribe({
        next: (result) => {
          this.importResult.set(result);
          this.selectedExcelFile.set(null);
          this.companyService.getCompanies();
          const id = this.selectedCompanyId();
          if (id) this.companyService.getAddresses(id);
          this.ui.toast('კატალოგი ჩანაცვლდა');
        },
        error: () => this.importError.set('ფაილის ფორმატი არასწორია')
      });
  }

  formatSize(bytes: number): string {
    return bytes < 1024 * 1024 ? `${Math.ceil(bytes / 1024)} KB` : `${(bytes / 1024 / 1024).toFixed(1)} MB`;
  }
}
