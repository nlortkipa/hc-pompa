import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Operator { id: number; name: string; phone: string; active: boolean; }
interface Pump { id: number; plate: string; type: string; }
interface Company { id: number; name: string; }
interface Address { id: number; companyId: number; name: string; active: boolean; }
interface Operation {
  id: number; date: string; operatorId: number;
  companyId: number | null; addressId: number | null;
  pumpPlate: string; pumpType: string; concreteVolume: number;
  pumpRelocation: number; stationaryRelocation: number; hydraulicRelocation: number;
  stationaryReposition: number; idleTime: number; dismantleAssemble: number;
}

interface WizardState {
  step: number; date: string;
  companyId: number | null; addressId: number | null;
  pumpPlate: string; pumpType: string; concreteVolume: number;
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
  companies: Company[] = [
    { id: 1, name: 'კომპანია 1' },
    { id: 2, name: 'კომპანია 2' },
    { id: 3, name: 'კომპანია 3' },
  ];
  addresses: Address[] = [
    { id: 1, companyId: 1, name: 'ლოკაცია 1', active: true },
    { id: 2, companyId: 1, name: 'ლოკაცია 2', active: true },
    { id: 3, companyId: 1, name: 'ლოკაცია 3', active: false },
    { id: 4, companyId: 2, name: 'ლოკაცია 4', active: true },
    { id: 5, companyId: 2, name: 'ლოკაცია 5', active: true },
    { id: 6, companyId: 3, name: 'ლოკაცია 6', active: true },
  ];
  operations: Operation[] = [];
  nextOpId = 6;

  /* ── WIZARD ── */
  readonly STEPS = ['თარიღი', 'კომპანია', 'მისამართი', 'პომპა', 'ბეტონი', 'ოპერაციები', 'განხილვა'];

  wiz: WizardState = this.defaultWiz();

  readonly opItems: OpItem[] = [
    { key: 'pumpRelocation', label: 'Pump Relocation (გადადგომა)', unit: 'ცალი' },
    { key: 'stationaryRelocation', label: 'Stationary Relocation', unit: 'ცალი' },
    { key: 'hydraulicRelocation', label: 'Hydraulic Relocation', unit: 'ცალი' },
    { key: 'stationaryReposition', label: 'სტაციონარის გადაწყობა', unit: 'ცალი' },
    { key: 'idleTime', label: 'მოცდენა', unit: 'სთ' },
    { key: 'dismantleAssemble', label: 'Dismantle / Assemble', unit: 'ცალი' },
  ];

  toast = { msg: '', visible: false, isError: false };
  private toastTimer: any;

  /* ── REVIEW LABELS ── */
  readonly opLabels: Record<string, string> = {
    pumpRelocation: 'Pump Relocation',
    stationaryRelocation: 'Stationary Relocation',
    hydraulicRelocation: 'Hydraulic Relocation',
    stationaryReposition: 'გადაწყობა',
    idleTime: 'მოცდენა',
    dismantleAssemble: 'Dismantle/Assemble',
  };

  /* ── LIFECYCLE ── */
  ngOnInit(): void { this.resetWizard(); }

  /* ── HELPERS ── */
  private defaultWiz(): WizardState {
    return {
      step: 0,
      date: new Date().toISOString().split('T')[0],
      companyId: null, addressId: null,
      pumpPlate: '', pumpType: '',
      concreteVolume: 0,
      pumpRelocation: 0, stationaryRelocation: 0, hydraulicRelocation: 0,
      stationaryReposition: 0, idleTime: 0, dismantleAssemble: 0,
    };
  }

  resetWizard(): void { this.wiz = this.defaultWiz(); }

  get stepRange(): number[] { return this.STEPS.map((_, i) => i); }

  stepStatus(i: number): 'done' | 'active' | '' {
    if (i < this.wiz.step) return 'done';
    if (i === this.wiz.step) return 'active';
    return '';
  }

  get filteredAddresses(): Address[] {
    return this.addresses.filter(a => a.companyId === this.wiz.companyId && a.active);
  }

  get selectedCompany(): Company | undefined {
    return this.companies.find(c => c.id === this.wiz.companyId);
  }

  get selectedAddress(): Address | undefined {
    return this.addresses.find(a => a.id === this.wiz.addressId);
  }

  get reviewOps(): { label: string; val: number }[] {
    return this.opItems
      .filter(op => this.wiz[op.key] > 0)
      .map(op => ({ label: this.opLabels[op.key], val: this.wiz[op.key] }));
  }

  /* ── PUMP AUTO-DETECT ── */
  autoDetectPump(): void {
    const p = this.pumps.find(
      p => p.plate.toLowerCase() === this.wiz.pumpPlate.toLowerCase()
    );
    this.wiz.pumpType = p ? p.type : '';
  }

  /* ── OP TOGGLE ── */
  toggleOp(key: OpItem['key'], checked: boolean): void {
    this.wiz[key] = checked ? 1 : 0;
  }

  /* ── VALIDATION ── */
  private validate(): boolean {
    const s = this.wiz.step;
    if (s === 0 && !this.wiz.date) { this.showToast('თარიღი სავალდებულოა', true); return false; }
    if (s === 1 && !this.wiz.companyId) { this.showToast('კომპანია სავალდებულოა', true); return false; }
    if (s === 2 && !this.wiz.addressId) { this.showToast('მისამართი სავალდებულოა', true); return false; }
    if (s === 3 && !this.wiz.pumpPlate) { this.showToast('პომპის ნომერი სავალდებულოა', true); return false; }
    if (s === 4 && !this.wiz.concreteVolume) { this.showToast('ბეტონის მოცულობა სავალდებულოა', true); return false; }
    return true;
  }

  /* ── NAV ── */
  wizNext(): void { if (!this.validate()) return; this.wiz.step++; }
  wizBack(): void { this.wiz.step--; }

  wizSubmit(): void {
    const op: Operation = {
      id: this.nextOpId++,
      date: this.wiz.date,
      operatorId: 1,
      companyId: this.wiz.companyId,
      addressId: this.wiz.addressId,
      pumpPlate: this.wiz.pumpPlate,
      pumpType: this.wiz.pumpType,
      concreteVolume: this.wiz.concreteVolume,
      pumpRelocation: this.wiz.pumpRelocation,
      stationaryRelocation: this.wiz.stationaryRelocation,
      hydraulicRelocation: this.wiz.hydraulicRelocation,
      stationaryReposition: this.wiz.stationaryReposition,
      idleTime: this.wiz.idleTime,
      dismantleAssemble: this.wiz.dismantleAssemble,
    };
    this.operations.push(op);
    this.showToast('✔ ჩანაწერი შენახულია!');
    setTimeout(() => this.resetWizard(), 900);
  }

  /* ── TOAST ── */
  showToast(msg: string, isError = false): void {
    clearTimeout(this.toastTimer);
    this.toast = { msg, visible: true, isError };
    this.toastTimer = setTimeout(() => (this.toast.visible = false), 2800);
  }
}