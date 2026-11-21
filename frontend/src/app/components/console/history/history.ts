import { Component, OnInit, signal, ChangeDetectorRef, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PumpOperationService } from '../../../services/pump-operation-service';
import { CompanyService } from '../../../services/company-service';
import { UserService } from '../../../services/user-service';
import { ProfileService } from '../../../services/profile-service';
import { PlateService } from '../../../services/plate-service';
import { PumpOperationFilter, PumpOperationModel } from '../../../models/pump-operation';
import { Plate } from '../../../models/plate-model';
import { UserModel } from '../../../models/user-model';
import { finalize } from 'rxjs';
import { SearchableSelect, SearchableSelectOption } from '../../searchable-select/searchable-select';
import { RecordDetails } from './record-details/record-details';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchableSelect, RecordDetails],
  templateUrl: './history.html',
  styleUrls: ['./history.scss'],
})
export class History implements OnInit {
  records = signal<PumpOperationModel[]>([]);
  selectedRecord = signal<PumpOperationModel | null>(null);
  isLoading = false;
  isExporting = false;
  
  canViewDetails = computed(() => {
    const profile = this.profileService.profile();
    return profile?.role === 'Admin' || profile?.role === 'Accountant';
  });

  users = signal<UserModel[]>([]);
  plates = signal<Plate[]>([]);
  selectedPlateId: number | null = null;
  companyOptions = computed<SearchableSelectOption[]>(() =>
    this.companyService.companies().map(company => ({
      value: company.id,
      label: company.name
    }))
  );
  operatorOptions = computed<SearchableSelectOption[]>(() =>
    this.users().map(user => ({
      value: user.id,
      label: `${user.firstName} ${user.lastName}`.trim()
    }))
  );
  plateOptions = computed<SearchableSelectOption[]>(() =>
    this.plates().map(plate => ({
      value: plate.id,
      label: plate.number
    }))
  );
  acceptedRecords = computed(() =>
    this.records().filter(record => this.reviewStatusValue(record) === 'Accepted')
  );
  totals = computed(() => {
    const result = {
      concreteVolume: 0,
      additionalStanding: 0,
      dismantleAssemble: 0,
      idleCount: 0
    };

    for (const record of this.records()) {
      result.concreteVolume += this.toNumber(record.concreteVolume);
      result.additionalStanding += this.toNumber(record.additionalStanding);
      result.dismantleAssemble += this.toNumber(record.dismantleAssemble);
      if (record.isIdle) result.idleCount++;
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
    fromDate: this.getCurrentMonthStart(),
    toDate: '',
    companyId: null as number | null,
    companyName: '',
    operatorId: null as number | null,
    operatorName: '',
    plateNumber: ''
  };

  constructor(
    private pumpOperationService: PumpOperationService,
    public companyService: CompanyService,
    private userService: UserService,
    private plateService: PlateService,
    private profileService: ProfileService,
    private cdr: ChangeDetectorRef
  ) {
    effect(() => {
      if (this.canViewDetails()) {
        this.userService.getAll().subscribe(data => this.users.set(data));
        this.plateService.getPlates().subscribe({
          next: data => this.plates.set(data),
          error: () => this.plates.set([])
        });
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
    this.selectedPlateId = null;
    this.filter = {
      fromDate: this.getCurrentMonthStart(),
      toDate: '',
      companyId: null,
      companyName: '',
      operatorId: null,
      operatorName: '',
      plateNumber: ''
    };
    this.loadRecords();
  }

  selectPlate(plateId: number | null): void {
    this.selectedPlateId = plateId;
    this.filter.plateNumber = this.plates().find(plate => plate.id === plateId)?.number ?? '';
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

  hasNote(record: PumpOperationModel): boolean {
    return !!record.note?.trim();
  }

  openRecord(record: PumpOperationModel): void {
    this.selectedRecord.set(record);
  }

  closeRecord(): void {
    this.selectedRecord.set(null);
  }

  distanceRangeLabel(record: PumpOperationModel): string {
    if (record.pumpType !== '<60მ') return '—';
    return record.distanceRange || '60მ-მდე';
  }

  private buildFilter(): PumpOperationFilter {
    const f: PumpOperationFilter = {};
    if (this.filter.fromDate) f.fromDate = this.filter.fromDate;
    if (this.filter.toDate) f.toDate = this.filter.toDate;
    if (this.filter.companyId != null) f.companyId = this.filter.companyId;
    if (this.filter.companyName) f.companyName = this.filter.companyName;
    if (this.filter.operatorId != null) f.operatorId = this.filter.operatorId;
    if (this.filter.operatorName) f.operatorName = this.filter.operatorName;
    if (this.filter.plateNumber) f.plateNumber = this.filter.plateNumber;

    return f;
  }

  private getCurrentMonthStart(): string {
    const now = new Date();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    return `${now.getFullYear()}-${month}-01`;
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
