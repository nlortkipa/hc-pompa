import { Component, NgZone, OnInit, ChangeDetectorRef, effect, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { CompanyModel } from '../../../models/company-model';
import { AddressModel } from '../../../models/address-model';
import { CompanyService } from '../../../services/company-service';
import { PlateService } from '../../../services/plate-service';
import { PumpOperationService } from '../../../services/pump-operation-service';
import { PumpOperationModel } from '../../../models/pump-operation';
import { Plate } from '../../../models/plate-model';
import { ProfileService } from '../../../services/profile-service';
import { Router } from '@angular/router';

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

type DropdownName = 'company' | 'address' | 'plate';

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

  operations: Operation[] = [];
  nextOpId = 6;

  /* ── PERMISSION ── */
  // platePermission removed

  /* ── WIZARD ── */
  readonly STEPS = ['თარიღი', 'კომპანია', 'მისამართი', 'პომპა', 'ბეტონი', 'ოპერაციები', 'განხილვა'];

  companySearch = signal('');
  addressSearch = signal('');
  plateSearch = signal('');
  openPicker = signal<DropdownName | null>(null);
  plates = signal<Plate[]>([]);

  filteredCompanies = computed(() => {
    const search = this.companySearch().toLowerCase();
    return this.companyService.companies().filter(c => 
      c.name.toLowerCase().includes(search)
    );
  });

  filteredPlates = computed(() => {
    const search = this.plateSearch().toLowerCase();
    return this.plates().filter(p => 
      p.number.toLowerCase().includes(search)
    );
  });

  wiz: WizardState = this.defaultWiz();
  isLoading = false;

  readonly opItems: OpItem[] = [
    { key: 'pumpRelocation',        label: 'პომპის გადადგმა',             unit: 'ცალი' },
    { key: 'stationaryRelocation',  label: 'სტაციონარის გადადგმა',        unit: 'ცალი' },
    { key: 'hydraulicRelocation',   label: 'ჰიდრავლიკის გადადგმა',        unit: 'ცალი' },
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
    pumpRelocation:       'პომპის გადადგმა',
    stationaryRelocation: 'სტაციონარის გადადგმა',
    hydraulicRelocation:  'ჰიდრავლიკის გადადგმა',
    stationaryReposition: 'გადაწყობა',
    idleTime:             'მოცდენა',
    dismantleAssemble:    'დაშლა / აწყობა',
  };

  constructor(
    public companyService: CompanyService,
    private plateService: PlateService,
    private pumpOperationService: PumpOperationService,
    private profileService: ProfileService,
    private router: Router,
    private zone: NgZone,
    private cdr: ChangeDetectorRef,
  ) {
    effect(() => {
      const profile = this.profileService.profile();
      if (profile && profile.role === 'Accountant') {
        this.router.navigate(['/console/history']);
      }
    });
  }

  ngOnInit(): void {
    this.resetWizard();
    this.plateService.getPlates().subscribe(res => {
      this.plates.set(res);
    });
  }

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
    this.companySearch.set('');
    this.addressSearch.set('');
    this.plateSearch.set('');
    this.closeDropdown();
    // this.platePermission = null;
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
    this.companySearch.set('');
    this.addressSearch.set('');
    this.clearError('companyId');
    if (this.wiz.companyId !== null) {
      this.companyService.getAddresses(this.wiz.companyId);
    } else {
      this.companyService.addresses.set([]);
    }
  }

  selectCompany(id: number): void {
    this.wiz.companyId = id;
    this.onCompanyChange();
    this.closeDropdown();
  }

  selectAddress(id: number): void {
    this.wiz.addressId = id;
    this.addressSearch.set('');
    this.clearError('addressId');
    this.closeDropdown();
  }

  selectPlate(number: string): void {
    this.wiz.pumpPlate = number;
    this.onPlateChange();
    this.closeDropdown();
  }

  openDropdown(name: DropdownName): void {
    this.openPicker.set(name);
  }

  closeDropdown(): void {
    this.openPicker.set(null);
  }

  isDropdownOpen(name: DropdownName): boolean {
    return this.openPicker() === name;
  }

  onPlateChange(): void {
    const plate = this.plates().find(p => p.number === this.wiz.pumpPlate);
    this.wiz.pumpId = plate?.id ?? null;
    this.plateSearch.set('');
    this.clearError('pumpPlate');
  }

  get filteredAddresses(): AddressModel[] { 
    const search = this.addressSearch().toLowerCase();
    return this.companyService.addresses().filter(a => 
      a.name.toLowerCase().includes(search)
    ); 
  }
  get selectedCompany(): CompanyModel | undefined { return this.companyService.companies().find(c => c.id === this.wiz.companyId); }
  get selectedAddress(): AddressModel | undefined { return this.companyService.addresses().find(a => a.id === this.wiz.addressId); }
  get selectedCompanyName(): string { return this.selectedCompany?.name ?? ''; }
  get selectedAddressName(): string { return this.selectedAddress?.name ?? ''; }

  get reviewOps(): { label: string; val: number; unit: string }[] {
    return this.opItems
      .filter(op => this.wiz[op.key] > 0)
      .map(op => ({ label: this.opLabels[op.key], val: this.wiz[op.key], unit: op.unit }));
  }

  onOpQtyChange(key: OpItem['key'], val: number): void {
    const min = key === 'idleTime' ? 0.5 : 0;
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
      if (this.wiz.concreteVolume === null || this.wiz.concreteVolume <= 0) {
        this.errors['concreteVolume'] = 'ბეტონის მოცულობა სავალდებულოა';
      }
    }

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }

  wizNext(): void {
    if (this.isLoading) return;
    if (!this.validate()) return;

    this.wiz.step++;
  }


  wizBack(): void { this.errors = {}; this.wiz.step--; }

  wizSubmit(): void {
    if (this.isLoading) return;
    this.isLoading = true;

    const model: PumpOperationModel = {
      id: 0,
      operatorId: 0,
      operatorFirstName: '',
      operatorLastName: '',
      companyName: '',
      addressName: '',
      date: new Date(this.wiz.date),
      companyId: this.wiz.companyId || 0,
      addressId: this.wiz.addressId || 0,
      concreteVolume: this.wiz.concreteVolume || 0,
      pumpRelocation: this.wiz.pumpRelocation,
      stationaryRelocation: this.wiz.stationaryRelocation,
      hydraulicRelocation: this.wiz.hydraulicRelocation,
      stationaryReposition: this.wiz.stationaryReposition,
      dismantleAssemble: this.wiz.dismantleAssemble,
      idleTime: this.wiz.idleTime || 0,
      plateNumber: this.wiz.pumpPlate
    };

    this.pumpOperationService.create(model)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (res: any) => {
          this.zone.run(() => {
            this.showToast('✔ ჩანაწერი შენახულია!');
            setTimeout(() => this.resetWizard(), 900);
          });
        },
        error: (err: any) => {
          this.zone.run(() => {
            console.error('ოპერაციის შენახვა ვერ მოხერხდა:', err);
            this.showToast('❌ შენახვა ვერ მოხერხდა');
          });
        }
      });
  }

  showToast(msg: string): void {
    clearTimeout(this.toastTimer);
    this.toast = { msg, visible: true };
    this.toastTimer = setTimeout(() => (this.toast.visible = false), 2800);
  }
}
