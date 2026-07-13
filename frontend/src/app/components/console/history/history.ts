import { Component, OnInit, signal, ChangeDetectorRef, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PumpOperationService } from '../../../services/pump-operation-service';
import { CompanyService } from '../../../services/company-service';
import { UserService } from '../../../services/user-service';
import { ProfileService } from '../../../services/profile-service';
import { PumpOperationFilter, PumpOperationModel } from '../../../models/pump-operation';
import { CompanyModel } from '../../../models/company-model';
import { UserModel } from '../../../models/user-model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './history.html',
  styleUrls: ['./history.scss'],
})
export class History implements OnInit {
  records = signal<PumpOperationModel[]>([]);
  isLoading = false;
  isExporting = false;
  
  canViewDetails = computed(() => {
    const profile = this.profileService.profile();
    return profile?.role === 'Admin' || profile?.role === 'Accountant';
  });

  users = signal<UserModel[]>([]);
  acceptedRecords = computed(() =>
    this.records().filter(record => this.reviewStatusValue(record) === 'Accepted')
  );
  totals = computed(() => {
    const result = {
      concreteVolume: 0,
      pumpRelocation: 0,
      stationaryRelocation: 0,
      hydraulicRelocation: 0,
      stationaryReposition: 0,
      dismantleAssemble: 0,
      idleTime: 0
    };

    for (const record of this.acceptedRecords()) {
      result.concreteVolume += this.toNumber(record.concreteVolume);
      result.pumpRelocation += this.toNumber(record.pumpRelocation);
      result.stationaryRelocation += this.toNumber(record.stationaryRelocation);
      result.hydraulicRelocation += this.toNumber(record.hydraulicRelocation);
      result.stationaryReposition += this.toNumber(record.stationaryReposition);
      result.dismantleAssemble += this.toNumber(record.dismantleAssemble);
      result.idleTime += this.toNumber(record.idleTime);
    }

    return result;
  });
  pendingCount = computed(() =>
    this.records().filter(record => this.reviewStatusValue(record) === 'Pending').length
  );
  acceptedCount = computed(() =>
    this.acceptedRecords().length
  );

  filter = {
    fromDate: '',
    toDate: '',
    companyId: null as number | null,
    operatorId: null as number | null,
    plateNumber: ''
  };

  constructor(
    private pumpOperationService: PumpOperationService,
    public companyService: CompanyService,
    private userService: UserService,
    private profileService: ProfileService,
    private cdr: ChangeDetectorRef
  ) {
    effect(() => {
      if (this.canViewDetails()) {
        this.userService.getAll().subscribe(data => this.users.set(data));
      }
    });
  }

  ngOnInit(): void {
    this.loadRecords();
  }

  loadRecords(): void {
    this.isLoading = true;
    const f = this.buildFilter();

    this.pumpOperationService.getAll(f)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (data) => this.records.set(data),
        error: (err) => console.error('ჩანაწერების ჩატვირთვა ვერ მოხერხდა:', err)
      });
  }

  clearFilters(): void {
    this.filter = {
      fromDate: '',
      toDate: '',
      companyId: null,
      operatorId: null,
      plateNumber: ''
    };
    this.loadRecords();
  }

  exportExcel(): void {
    if (this.isExporting) return;

    this.isExporting = true;

    this.pumpOperationService.downloadExcel(this.buildFilter())
      .pipe(finalize(() => {
        this.isExporting = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (blob) => this.saveExcelFile(blob),
        error: (err) => console.error('ჩანაწერების ექსპორტი ვერ მოხერხდა:', err)
      });
  }

  reviewStatusValue(record: PumpOperationModel): string {
    return record.reviewStatus || 'Pending';
  }

  reviewStatusLabel(record: PumpOperationModel): string {
    const status = this.reviewStatusValue(record);
    if (status === 'Accepted') return 'დადასტურებული';
    if (status === 'Rejected') return 'უარყოფილი';
    return 'მოლოდინში';
  }

  reviewStatusClass(record: PumpOperationModel): string {
    if (record.reviewStatus === 'Accepted') return 'badge-green';
    if (record.reviewStatus === 'Rejected') return 'badge-red';
    return 'badge-yellow';
  }

  private buildFilter(): PumpOperationFilter {
    const f: PumpOperationFilter = {};
    if (this.filter.fromDate) f.fromDate = this.filter.fromDate;
    if (this.filter.toDate) f.toDate = this.filter.toDate;
    if (this.filter.companyId != null) f.companyId = this.filter.companyId;
    if (this.filter.operatorId != null) f.operatorId = this.filter.operatorId;
    if (this.filter.plateNumber) f.plateNumber = this.filter.plateNumber;

    return f;
  }

  private toNumber(value: number | null | undefined): number {
    const numericValue = Number(value ?? 0);
    return Number.isFinite(numericValue) ? numericValue : 0;
  }

  private saveExcelFile(blob: Blob): void {
    const url = window.URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = `პომპის-ოპერაციები-${new Date().toISOString().slice(0, 10)}.xlsx`;
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
    window.URL.revokeObjectURL(url);
  }
}
