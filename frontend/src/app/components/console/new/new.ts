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
  additionalStanding: number; isIdle: boolean; dismantleAssemble: number;
}

interface WizardState {
  step: number; date: string;
  companyId: number | null; addressId: number | null;
  pumpId: number | null;
  pumpPlate: string; pumpType: string;
  concreteVolume: number | null;
  additionalStanding: number | null; isIdle: boolean; dismantleAssemble: number | null;
}

interface WizardStep {
  id: number;
  label: string;
}

interface OpItem {
  key: keyof Pick<WizardState,
    'additionalStanding' | 'dismantleAssemble'>;
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
  readonly ALL_STEPS: WizardStep[] = [
    { id: 0, label: 'თარიღი' },
    { id: 1, label: 'კომპანია' },
    { id: 2, label: 'მისამართი' },
    { id: 3, label: 'პომპა' },
    { id: 4, label: 'მოცდენა' },
    { id: 5, label: 'ბეტონი' },
    { id: 6, label: 'ოპერაციები' },
    { id: 7, label: 'განხილვა' },
  ];

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
    { key: 'additionalStanding', label: 'დამატებითი დგომა', unit: 'ცალი' },
    { key: 'dismantleAssemble',  label: 'დაშლა და აწყობა',   unit: 'ცალი' },
  ];

  /* ── INLINE ERRORS ── */
  errors: Record<string, string> = {};

  toast = { msg: '', visible: false };
  private toastTimer: any;

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
      additionalStanding: null, isIdle: false, dismantleAssemble: null,
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

  get steps(): WizardStep[] {
    return this.wiz.isIdle
      ? this.ALL_STEPS.filter(step => step.id !== 5 && step.id !== 6)
      : this.ALL_STEPS;
  }

  get currentStepPosition(): number {
    return this.steps.findIndex(step => step.id === this.wiz.step);
  }

  get currentStepLabel(): string {
    return this.ALL_STEPS.find(step => step.id === this.wiz.step)?.label ?? '';
  }

  get isLastStep(): boolean {
    return this.currentStepPosition === this.steps.length - 1;
  }

  stepStatus(stepId: number): 'done' | 'active' | '' {
    const stepPosition = this.steps.findIndex(step => step.id === stepId);
    if (stepPosition < this.currentStepPosition) return 'done';
    if (stepId === this.wiz.step) return 'active';
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
    this.wiz.pumpType = plate?.type ?? '';
    if (plate?.isStationary) {
      this.wiz.additionalStanding = null;
    }
    this.plateSearch.set('');
    this.clearError('pumpPlate');
  }

  get selectedPlate(): Plate | undefined {
    return this.plates().find(p => p.id === this.wiz.pumpId);
  }

  get visibleOpItems(): OpItem[] {
    return this.selectedPlate?.isStationary
      ? this.opItems.filter(op => op.key === 'dismantleAssemble')
      : this.opItems;
  }

  onIdleChange(): void {
    if (!this.wiz.isIdle) return;

    this.wiz.concreteVolume = null;
    this.wiz.additionalStanding = null;
    this.wiz.dismantleAssemble = null;
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

  get reviewOps(): { label: string; val: number | string; unit: string }[] {
    const operations: { label: string; val: number | string; unit: string }[] = [];

    if (!this.selectedPlate?.isStationary && (this.wiz.additionalStanding ?? 0) > 0) {
      operations.push({ label: 'დამატებითი დგომა', val: this.wiz.additionalStanding!, unit: 'ცალი' });
    }
    if ((this.wiz.dismantleAssemble ?? 0) > 0) {
      operations.push({ label: 'დაშლა და აწყობა', val: this.wiz.dismantleAssemble!, unit: 'ცალი' });
    }

    return operations;
  }

  onOpQtyChange(key: OpItem['key'], val: number): void {
    if (val < 0) this.wiz[key] = 0;
  }

  stripLeadingZeros(event: Event): void {
    const el = event.target as HTMLInputElement;
    const cleaned = el.value.replace(/^0+(?=\d)/, '');
    if (cleaned !== el.value) el.value = cleaned;
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

    if (s === 5) {
      if (this.wiz.concreteVolume === null) {
        this.errors['concreteVolume'] = 'ბეტონის მოცულობა სავალდებულოა';
      }
    }

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }

  wizNext(): void {
    if (this.isLoading) return;
    if (!this.validate()) return;

    const nextStep = this.steps[this.currentStepPosition + 1];
    if (nextStep) {
      this.wiz.step = nextStep.id;
    }
  }


  wizBack(): void {
    this.errors = {};
    const previousStep = this.steps[this.currentStepPosition - 1];
    if (previousStep) {
      this.wiz.step = previousStep.id;
    }
  }

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
      additionalStanding: this.wiz.additionalStanding ?? 0,
      dismantleAssemble: this.wiz.dismantleAssemble ?? 0,
      isIdle: this.wiz.isIdle,
      plateNumber: this.wiz.pumpPlate,
      pumpType: this.wiz.pumpType
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
