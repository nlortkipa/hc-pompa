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
import { Icon } from '../../icon/icon';
import { UiService } from '../../../services/ui.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchableSelect, RecordDetails, Icon],
  templateUrl: './history.html',
  styleUrls: ['./history.scss'],
})
export class History implements OnInit {
  records = signal<PumpOperationModel[]>([]);
  selectedRecord = signal<PumpOperationModel | null>(null);
  isLoading = false;
  isExporting = false;
  isDeleting = signal(false);
  hasLoaded = false;
  filtersOpen = false;
  datePreset: 'today' | 'month' | 'prev' | 'all' | 'custom' = 'today';
  
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
    fromDate: this.getToday(),
    toDate: this.getToday(),
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
    private ui: UiService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    effect(() => {
      if (this.canViewDetails()) {
        this.userService.getAll().subscribe(data => this.users.set(data));
      }
    });
    this.plateService.getPlates().subscribe({
      next: data => this.plates.set(data),
      error: () => this.plates.set([])
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
        this.hasLoaded = true;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (data) => this.records.set(data),
        error: (err) => {
          console.error('ჩანაწერების ჩატვირთვა ვერ მოხერხდა:', err);
          this.ui.toast('ჩანაწერების ჩატვირთვა ვერ მოხერხდა', 'error');
        }
      });
  }

  applyDatePreset(preset: 'today' | 'month' | 'prev' | 'all'): void {
    this.datePreset = preset;
    const now = new Date();
    if (preset === 'today') {
      this.filter.fromDate = this.getToday();
      this.filter.toDate = this.getToday();
    } else if (preset === 'month') {
      this.filter.fromDate = this.getCurrentMonthStart();
      this.filter.toDate = '';
    } else if (preset === 'prev') {
      const start = new Date(now.getFullYear(), now.getMonth() - 1, 1);
      const end = new Date(now.getFullYear(), now.getMonth(), 0);
      this.filter.fromDate = this.toIsoDate(start);
      this.filter.toDate = this.toIsoDate(end);
    } else {
      this.filter.fromDate = '';
      this.filter.toDate = '';
    }
    this.loadRecords();
  }

  onDateEdited(): void {
    this.datePreset = 'custom';
  }

  get activeFilterCount(): number {
    const f = this.filter;
    return [f.companyId ?? f.companyName, f.operatorId ?? f.operatorName, f.plateNumber].filter(v => v !== null && v !== '').length;
  }

  clearFilters(): void {
    this.datePreset = 'today';
    this.selectedPlateId = null;
    this.filter = {
      fromDate: this.getToday(),
      toDate: this.getToday(),
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
        next: (blob) => {
          this.saveExcelFile(blob);
          this.ui.toast('Excel ფაილი ჩამოიტვირთა');
        },
        error: (err) => {
          console.error('ჩანაწერების ექსპორტი ვერ მოხერხდა:', err);
          this.ui.toast('ექსპორტი ვერ მოხერხდა', 'error');
        }
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

  /** Mirrors the backend rule: only the creator, only while pending. */
  canModify(record: PumpOperationModel): boolean {
    const profile = this.profileService.profile();
    return !!profile
      && profile.role !== 'Accountant'
      && record.operatorId === profile.id
      && this.reviewStatusValue(record) === 'Pending';
  }

  editRecord(record: PumpOperationModel): void {
    this.router.navigate(['/console/new'], { queryParams: { edit: record.id } });
  }

  async deleteRecord(record: PumpOperationModel): Promise<void> {
    if (this.isDeleting()) return;

    const ok = await this.ui.confirm({
      title: 'ჩანაწერის წაშლა',
      message: 'ჩანაწერი წაიშლება და შემმოწმებლები ვეღარ ნახავენ. გსურთ გაგრძელება?',
      confirmLabel: 'წაშლა',
      destructive: true,
    });
    if (!ok) return;

    this.isDeleting.set(true);
    this.pumpOperationService.delete(record.id)
      .pipe(finalize(() => {
        this.isDeleting.set(false);
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: () => {
          this.records.update(list => list.filter(r => r.id !== record.id));
          this.closeRecord();
          this.ui.toast('ჩანაწერი წაიშალა');
        },
        error: (err) => {
          console.error('ჩანაწერის წაშლა ვერ მოხერხდა:', err);
          if (err?.status === 409) {
            this.ui.toast('ჩანაწერი უკვე შემოწმებულია და ვეღარ წაიშლება', 'error');
            this.closeRecord();
            this.loadRecords();
          } else {
            this.ui.toast('წაშლა ვერ მოხერხდა', 'error');
          }
        }
      });
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

  private getToday(): string {
    return this.toIsoDate(new Date());
  }

  private getCurrentMonthStart(): string {
    const now = new Date();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    return `${now.getFullYear()}-${month}-01`;
  }

  private toIsoDate(d: Date): string {
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
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
