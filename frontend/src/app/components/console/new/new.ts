import { Component, NgZone, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { CompanyModel } from '../../../models/company-model';
import { AddressModel } from '../../../models/address-model';
import { CompanyService } from '../../../services/company-service';
import { LicensePlatePermissionService } from '../../../services/license-plate-permission-service';
import { PumpOperationModel } from '../../../models/pump-operation';

interface Operator { id: number; name: string; phone: string; active: boolean; }
interface Pump { id: number; plate: string; type: string; }
interface Operation {
  id: number; date: string; operatorId: number;
  companyId: number | null; addressId: number | null;
  pumpId: number | null;
  pumpPlate: string; pumpType: string; concreteVolume: number;
  pumpRelocation: number; stationaryRelocation: number; hydraulicRelocation: number;
  stationaryReposition: number; idleTime: number; dismantleAssemble: number;
}

interface WizardState {
  step: number; date: string;
  companyId: number | null; addressId: number | null;
  pumpId: number | null;
  pumpPlate: string; pumpType: string;
  concreteVolume: number | null;
  pumpRelocation: number; stationaryRelocation: number; hydraulicRelocation: number;
  stationaryReposition: number; idleTime: number; dismantleAssemble: number;
}

interface OpItem {
  key: keyof Pick<WizardState,
    'pumpRelocation' | 'stationaryRelocation' | 'hydraulicRelocation' |
    'stationaryReposition' | 'idleTime' | 'dismantleAssemble'>;
  label: string;
  unit: string;
}

@Component({
  selector: 'app-pompa-wizard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './new.html',
  styleUrls: ['./new.scss'],
})
export class New implements OnInit {

  /* ── DATA ── */
  operators: Operator[] = [
    { id: 1, name: 'გიორგი მამულაშვილი 67', phone: '+995 599 100 001', active: true },
    { id: 2, name: 'ლევან ბერიძე', phone: '+995 577 200 002', active: true },
    { id: 3, name: 'დავით ჩიქოვანი', phone: '+995 598 300 003', active: true },
  ];
  pumps: Pump[] = [
    { id: 1, plate: 'GE-001-AA', type: 'Pump' },
    { id: 2, plate: 'GE-002-BB', type: 'Stationary Pump' },
    { id: 3, plate: 'GE-003-CC', type: 'Hydraulic Distributor' },
  ];

  operations: Operation[] = [];
  nextOpId = 6;

  /* ── PERMISSION ── */
  platePermission: number | null = null;

  /* ── WIZARD ── */
  readonly STEPS = ['თარიღი', 'კომპანია', 'მისამართი', 'პომპა', 'ბეტონი', 'ოპერაციები', 'განხილვა'];

  wiz: WizardState = this.defaultWiz();
  isLoading = false;

  readonly opItems: OpItem[] = [
    { key: 'pumpRelocation',        label: 'Pump Relocation (გადადგომა)', unit: 'ცალი' },
    { key: 'stationaryRelocation',  label: 'Stationary Relocation',       unit: 'ცალი' },
    { key: 'hydraulicRelocation',   label: 'Hydraulic Relocation',        unit: 'ცალი' },
    { key: 'stationaryReposition',  label: 'სტაციონარის გადაწყობა',       unit: 'ცალი' },
    { key: 'idleTime',              label: 'მოცდენა',                     unit: 'სთ'   },
    { key: 'dismantleAssemble',     label: 'დაშლა / აწყობა',        unit: 'ცალი' },
  ];

  /* ── INLINE ERRORS ── */
  errors: Record<string, string> = {};

  toast = { msg: '', visible: false };
  private toastTimer: any;

  /* ── REVIEW LABELS ── */
  readonly opLabels: Record<string, string> = {
    pumpRelocation:       'Pump Relocation',
    stationaryRelocation: 'Stationary Relocation',
    hydraulicRelocation:  'Hydraulic Relocation',
    stationaryReposition: 'გადაწყობა',
    idleTime:             'მოცდენა',
    dismantleAssemble:    'Dismantle/Assemble',
  };

  constructor(
    public companyService: CompanyService,
    private licensePlatePermissionService: LicensePlatePermissionService,
    private zone: NgZone,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void { this.resetWizard(); }

  private defaultWiz(): WizardState {
    return {
      step: 0,
      date: new Date().toISOString().split('T')[0],
      companyId: null, addressId: null,
      pumpId: null,
      pumpPlate: '', pumpType: '',
      concreteVolume: null,
      pumpRelocation: 0, stationaryRelocation: 0, hydraulicRelocation: 0,
      stationaryReposition: 0, idleTime: 0, dismantleAssemble: 0,
    };
  }

  resetWizard(): void {
    this.wiz = this.defaultWiz();
    this.errors = {};
    this.platePermission = null;
    this.companyService.addresses.set([]);
  }

  clearError(field: string): void {
    if (this.errors[field]) {
      delete this.errors[field];
      this.cdr.detectChanges();
    }
  }

  get stepRange(): number[] { return this.STEPS.map((_, i) => i); }

  stepStatus(i: number): 'done' | 'active' | '' {
    if (i < this.wiz.step) return 'done';
    if (i === this.wiz.step) return 'active';
    return '';
  }

  onCompanyChange(): void {
    this.wiz.addressId = null;
    this.clearError('companyId');
    if (this.wiz.companyId !== null) {
      this.companyService.getAddresses(this.wiz.companyId);
    } else {
      this.companyService.addresses.set([]);
    }
  }

  get filteredAddresses(): AddressModel[] { return this.companyService.addresses(); }
  get selectedCompany(): CompanyModel | undefined { return this.companyService.companies().find(c => c.id === this.wiz.companyId); }
  get selectedAddress(): AddressModel | undefined { return this.companyService.addresses().find(a => a.id === this.wiz.addressId); }

  get reviewOps(): { label: string; val: number }[] {
    return this.opItems
      .filter(op => this.wiz[op.key] > 0)
      .map(op => ({ label: this.opLabels[op.key], val: this.wiz[op.key] }));
  }

  autoDetectPump(): void {
    const p = this.pumps.find(p => p.plate.toLowerCase() === this.wiz.pumpPlate.toLowerCase());
    this.wiz.pumpType = p ? p.type : '';
    this.wiz.pumpId = p ? p.id : null;
    this.clearError('pumpPlate');
  }

  toggleOp(key: OpItem['key'], checked: boolean): void {
    this.wiz[key] = checked ? 1 : 0;
  }

  onOpQtyChange(key: OpItem['key'], val: number): void {
    const min = key === 'idleTime' ? 0.5 : 1;
    if (val < min) this.wiz[key] = min;
  }

  onConcreteVolumeChange(): void {
    this.clearError('concreteVolume');
    if (this.wiz.concreteVolume !== null && this.wiz.concreteVolume < 0) {
      this.wiz.concreteVolume = 0;
    }
  }

  setConcreteVolume(v: number): void {
    this.wiz.concreteVolume = v;
    this.onConcreteVolumeChange();
  }

  private validate(): boolean {
    this.errors = {};
    const s = this.wiz.step;

    if (s === 0) {
      if (!this.wiz.date) {
        this.errors['date'] = 'თარიღი სავალდებულოა';
      } else {
        const selected = new Date(this.wiz.date);
        const today = new Date();
        if (selected > today) {
          this.errors['date'] = 'თარიღი არ შეიძლება იყოს მომავალში';
        } else if (
          selected.getFullYear() !== today.getFullYear() ||
          selected.getMonth() !== today.getMonth()
        ) {
          this.errors['date'] = 'თარიღი მიმდინარე თვეში უნდა იყოს';
        }
      }
    }

    if (s === 1 && !this.wiz.companyId) {
      this.errors['companyId'] = 'კომპანია სავალდებულოა';
    }
    if (s === 2 && !this.wiz.addressId) {
      this.errors['addressId'] = 'მისამართი სავალდებულოა';
    }
    if (s === 3 && !this.wiz.pumpPlate.trim()) {
      this.errors['pumpPlate'] = 'პომპის ნომერი სავალდებულოა';
    }

    if (s === 4) {
      if (this.wiz.concreteVolume === null || this.wiz.concreteVolume < 0) {
        this.errors['concreteVolume'] = 'ბეტონის მოცულობა სავალდებულოა';
      } else if (
        this.platePermission !== null &&
        this.wiz.concreteVolume > this.platePermission
      ) {
        this.errors['concreteVolume'] =
          `მაქსიმუმი ${this.platePermission} m³ (ნებართვის მიხედვით)`;
      }
    }

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }

  wizNext(): void {
    if (this.isLoading) return;
    if (!this.validate()) return;

    if (this.wiz.step === 3) {
      this.isLoading = true;
      this.licensePlatePermissionService
        .getPlatePermission(this.wiz.pumpPlate.trim())
        .pipe(finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }))
        .subscribe({
          next: (perm) => {
            this.zone.run(() => {
              if (perm == null) {
                this.errors['pumpPlate'] = 'სანომრე ნიშანი სისტემაში არ მოიძებნა';
                return;
              }
              this.platePermission = perm.permission ?? null;
              if (this.platePermission === 0) {
                 this.errors['pumpPlate'] = 'ამ პომპაზე ბეტონის მოცულობის დაშვება არის 0 m³';
                 return;
              }
              this.wiz.step++;
            });
          },
          error: (err) => {
            this.zone.run(() => {
              console.warn('Plate lookup failed (likely 404):', err);
              this.errors['pumpPlate'] = 'სანომრე ნიშანი სისტემაში არ მოიძებნა';
              this.cdr.detectChanges();
            });
          },
        });
      return;
    }

    this.wiz.step++;
  }

  wizBack(): void { this.errors = {}; this.wiz.step--; }

  wizSubmit(): void {
    const op: Operation = {
      id: this.nextOpId++,
      date: this.wiz.date,
      operatorId: 1,
      companyId: this.wiz.companyId,
      addressId: this.wiz.addressId,
      pumpId: this.wiz.pumpId,
      pumpPlate: this.wiz.pumpPlate,
      pumpType: this.wiz.pumpType,
      concreteVolume: this.wiz.concreteVolume ?? 0,
      pumpRelocation: this.wiz.pumpRelocation,
      stationaryRelocation: this.wiz.stationaryRelocation,
      hydraulicRelocation: this.wiz.hydraulicRelocation,
      stationaryReposition: this.wiz.stationaryReposition,
      idleTime: this.wiz.idleTime,
      dismantleAssemble: this.wiz.dismantleAssemble,
    };
    this.operations.push(op);

    // Create and log PumpOperationModel
    const model: PumpOperationModel = {
      date: new Date(this.wiz.date),
      companyId: this.wiz.companyId || 0,
      addressId: this.wiz.addressId || 0,
      concreteVolume: this.wiz.concreteVolume || 0,
      pumpRelocation: this.wiz.pumpRelocation,
      stationaryRelocation: this.wiz.stationaryRelocation,
      hydraulicRelocation: this.wiz.hydraulicRelocation,
      dismantleAssemble: this.wiz.dismantleAssemble
    };
    console.log('Final PumpOperationModel:', model);

    this.showToast('✔ ჩანაწერი შენახულია!');
    setTimeout(() => this.resetWizard(), 900);
  }

  showToast(msg: string): void {
    clearTimeout(this.toastTimer);
    this.toast = { msg, visible: true };
    this.toastTimer = setTimeout(() => (this.toast.visible = false), 2800);
  }
}