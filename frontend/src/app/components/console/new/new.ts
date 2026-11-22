import { Component, DestroyRef, ElementRef, inject, HostListener, NgZone, OnInit, ChangeDetectorRef, effect, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { CompanyModel } from '../../../models/company-model';
import { AddressModel } from '../../../models/address-model';
import { CompanyService } from '../../../services/company-service';
import { PlateService } from '../../../services/plate-service';
import { PumpOperationService } from '../../../services/pump-operation-service';
import { PumpOperationModel } from '../../../models/pump-operation';
import { Plate } from '../../../models/plate-model';
import { ProfileService } from '../../../services/profile-service';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Icon } from '../../icon/icon';
import { UiService } from '../../../services/ui.service';

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
  isDaily: boolean;
  note: string;
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
  imports: [CommonModule, FormsModule, Icon],
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
  /** Id of the pending operation being edited (from ?edit=), or null when creating. */
  editId = signal<number | null>(null);
  isLoadingRecord = signal(false);
  readonly NOTE_MAX_LENGTH = 500;
  private destroyRef = inject(DestroyRef);

  readonly opItems: OpItem[] = [
    { key: 'additionalStanding', label: 'დამატებითი დგომა', unit: 'ცალი' },
    { key: 'dismantleAssemble',  label: 'დაშლა და აწყობა',   unit: 'ცალი' },
  ];

  /* ── INLINE ERRORS ── */
  errors: Record<string, string> = {};

  readonly concretePresets = [5, 7, 9, 10, 12];
  private readonly finePointer = typeof window !== 'undefined' && window.matchMedia?.('(pointer: fine)').matches;

  constructor(
    public companyService: CompanyService,
    private plateService: PlateService,
    private pumpOperationService: PumpOperationService,
    private profileService: ProfileService,
    private router: Router,
    private route: ActivatedRoute,
    private zone: NgZone,
    private cdr: ChangeDetectorRef,
    private ui: UiService,
    private host: ElementRef<HTMLElement>,
  ) {
    effect(() => {
      const profile = this.profileService.profile();
      if (profile && profile.role === 'Accountant') {
        this.router.navigate(['/console/history']);
      }
    });
  }

  ngOnInit(): void {
    this.companyService.getCompanies();
    this.resetWizard();
    this.focusStep();

    // Query-param subscription (not a snapshot) so switching between edit and the plain
    // "new" link re-initialises the wizard even though the component is reused.
    this.route.queryParamMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      const editId = Number(params.get('edit'));
      if (Number.isInteger(editId) && editId > 0) {
        this.startEdit(editId);
      } else {
        this.editId.set(null);
        this.resetWizard();
        this.loadPlates();
      }
    });
  }

  private loadPlates(): void {
    this.plateService.getPlates().subscribe(res => {
      this.plates.set(res);
    });
  }

  private startEdit(id: number): void {
    this.editId.set(id);
    this.resetWizard();
    this.isLoadingRecord.set(true);

    forkJoin({ record: this.pumpOperationService.getById(id), plates: this.plateService.getPlates() })
      .pipe(finalize(() => {
        this.isLoadingRecord.set(false);
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: ({ record, plates }) => {
          this.plates.set(plates);
          if ((record.reviewStatus || 'Pending') !== 'Pending') {
            this.ui.toast('შემოწმებული ჩანაწერის რედაქტირება შეუძლებელია', 'error');
            this.router.navigate(['/console/history']);
            return;
          }
          this.prefill(record);
        },
        error: (err) => {
          console.error('ჩანაწერის ჩატვირთვა ვერ მოხერხდა:', err);
          this.ui.toast('ჩანაწერის ჩატვირთვა ვერ მოხერხდა', 'error');
          this.router.navigate(['/console/history']);
        }
      });
  }

  private prefill(record: PumpOperationModel): void {
    const plate = this.plates().find(p => p.number.toLowerCase() === record.plateNumber.toLowerCase());
    // 0 means "not entered" in the wizard, which uses null for empty quantity inputs.
    const qty = (value: number | null | undefined) => (value ? value : null);

    this.wiz = {
      ...this.defaultWiz(),
      // Go straight to the summary; the user steps back to whatever they want to change.
      step: 7,
      date: String(record.date).slice(0, 10),
      companyId: record.companyId,
      addressId: record.addressId,
      pumpId: plate?.id ?? null,
      pumpPlate: plate?.number ?? record.plateNumber,
      pumpType: plate?.type ?? record.pumpType,
      isIdle: record.isIdle,
      isDaily: record.isDaily,
      concreteVolume: record.isIdle ? null : record.concreteVolume,
      additionalStanding: qty(record.additionalStanding),
      dismantleAssemble: qty(record.dismantleAssemble),
      note: record.note ?? '',
    };
    this.companyService.getAddresses(record.companyId);

    if (!plate) {
      // The plate was renamed or deleted since the report was made; the server would
      // reject the old number, so make the user pick a current one.
      this.wiz.step = 3;
      this.wiz.pumpPlate = '';
      this.wiz.pumpId = null;
      this.wiz.pumpType = '';
      this.errors['pumpPlate'] = `პომპა ${record.plateNumber} აღარ არსებობს. აირჩიეთ პომპა.`;
    }
  }

  /** Georgian text for the reasons the API rejects a create/edit with 400. */
  private badRequestMessage(err: any): string | null {
    const raw = typeof err?.error === 'string' ? err.error : '';
    if (raw.includes('Plate was not found')) return 'არჩეული პომპა აღარ არსებობს. აირჩიეთ პომპა.';
    if (raw.includes('checker')) return 'ამ პომპას შემმოწმებელი არ ჰყავს. დაამატეთ შემმოწმებელი პომპების გვერდზე.';
    if (raw.includes('Note cannot be longer')) return `შენიშვნა არ უნდა აღემატებოდეს ${this.NOTE_MAX_LENGTH} სიმბოლოს.`;
    return null;
  }

  cancelEdit(): void {
    this.router.navigate(['/console/history']);
  }

  private defaultWiz(): WizardState {
    return {
      step: 0,
      date: this.toLocalIsoDate(new Date()),
      companyId: null, addressId: null,
      pumpId: null,
      pumpPlate: '', pumpType: '',
      concreteVolume: null,
      additionalStanding: null, isIdle: false, dismantleAssemble: null,
      isDaily: false,
      note: '',
    };
  }

  /** YYYY-MM-DD in the user's local timezone (toISOString() would use UTC). */
  private toLocalIsoDate(d: Date): string {
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
  }

  private parseLocalDate(value: string): Date {
    const [y, m, d] = value.split('-').map(Number);
    return new Date(y, m - 1, d);
  }

  get todayIso(): string {
    return this.toLocalIsoDate(new Date());
  }

  get monthStartIso(): string {
    const d = new Date();
    return this.toLocalIsoDate(new Date(d.getFullYear(), d.getMonth(), 1));
  }

  get formattedDate(): string {
    if (!this.wiz.date) return '';
    const d = this.parseLocalDate(this.wiz.date);
    const months = ['იანვარი', 'თებერვალი', 'მარტი', 'აპრილი', 'მაისი', 'ივნისი',
      'ივლისი', 'აგვისტო', 'სექტემბერი', 'ოქტომბერი', 'ნოემბერი', 'დეკემბერი'];
    return `${d.getDate()} ${months[d.getMonth()]}, ${d.getFullYear()}`;
  }

  /** Enter advances the wizard (except inside the note field). */
  @HostListener('keydown.enter', ['$event'])
  onEnter(event: Event): void {
    const target = event.target as HTMLElement;
    if (target.tagName === 'TEXTAREA' || target.tagName === 'BUTTON') return;
    if (target.classList.contains('dropdown-search')) {
      event.preventDefault();
      this.pickFirstMatch();
      return;
    }
    event.preventDefault();
    if (this.isLastStep) this.wizSubmit();
    else this.wizNext();
  }

  private pickFirstMatch(): void {
    const step = this.wiz.step;
    if (step === 1 && this.filteredCompanies()[0]) this.selectCompany(this.filteredCompanies()[0].id);
    else if (step === 2 && this.filteredAddresses[0]) this.selectAddress(this.filteredAddresses[0].id);
    else if (step === 3 && this.filteredPlates()[0]) this.selectPlate(this.filteredPlates()[0].number);
  }

  /** On desktop, put the caret in the first field of the new step. */
  private focusStep(): void {
    if (!this.finePointer) return;
    setTimeout(() => {
      const el = this.host.nativeElement.querySelector<HTMLElement>(
        '.wizard-content input:not([type=checkbox]), .wizard-content .dropdown-trigger, .wizard-content textarea'
      );
      el?.focus();
    });
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

  private readonly STEP_TITLES: Record<number, string> = {
    0: 'როდის შესრულდა ოპერაცია?',
    1: 'აირჩიეთ კომპანია',
    2: 'აირჩიეთ მისამართი',
    3: 'აირჩიეთ პომპა',
    4: 'იყო მოცდენა?',
    5: 'ბეტონის მოცულობა',
    6: 'დამატებითი ოპერაციები',
    7: 'გადახედეთ და შეინახეთ',
  };

  get stepTitle(): string {
    return this.STEP_TITLES[this.wiz.step] ?? this.currentStepLabel;
  }

  stepOp(key: OpItem['key'], delta: number): void {
    this.wiz[key] = Math.max(0, (this.wiz[key] ?? 0) + delta) || null;
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
    setTimeout(() => this.host.nativeElement.querySelector<HTMLElement>('.dropdown-search')?.focus());
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
    if (plate && !plate.isStationary) {
      this.wiz.dismantleAssemble = null;
    }
    this.plateSearch.set('');
    this.clearError('pumpPlate');
  }

  get selectedPlate(): Plate | undefined {
    return this.plates().find(p => p.id === this.wiz.pumpId);
  }

  get visibleOpItems(): OpItem[] {
    return this.selectedPlate?.isStationary
      ? this.opItems
      : this.opItems.filter(op => op.key === 'additionalStanding');
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

    if ((this.wiz.additionalStanding ?? 0) > 0) {
      operations.push({ label: 'დამატებითი დგომა', val: this.wiz.additionalStanding!, unit: 'ცალი' });
    }
    if (this.selectedPlate?.isStationary && (this.wiz.dismantleAssemble ?? 0) > 0) {
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
        const selected = this.parseLocalDate(this.wiz.date);
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
      this.focusStep();
    }
  }


  wizBack(): void {
    this.errors = {};
    const previousStep = this.steps[this.currentStepPosition - 1];
    if (previousStep) {
      this.wiz.step = previousStep.id;
      this.focusStep();
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
      isDaily: this.wiz.isDaily,
      plateNumber: this.wiz.pumpPlate,
      pumpType: this.wiz.pumpType,
      note: this.wiz.note.trim() || null
    };

    const editId = this.editId();
    const request$ = editId === null
      ? this.pumpOperationService.create(model)
      : this.pumpOperationService.update(editId, model);

    request$
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (res: any) => {
          this.zone.run(() => {
            if (editId !== null) {
              this.ui.toast('ცვლილებები შენახულია');
              this.router.navigate(['/console/history']);
              return;
            }
            this.ui.toast('ჩანაწერი შენახულია');
            this.resetWizard();
            this.focusStep();
          });
        },
        error: (err: any) => {
          this.zone.run(() => {
            console.error('ოპერაციის შენახვა ვერ მოხერხდა:', err);
            const reason = err?.status === 400 ? this.badRequestMessage(err) : null;
            if (reason) {
              if (reason.includes('პომპა')) {
                this.wiz.step = 3;
                this.errors = { pumpPlate: reason };
                this.focusStep();
              }
              this.ui.toast(reason, 'error', 5000);
              return;
            }
            if (editId !== null && (err?.status === 409 || err?.status === 404)) {
              this.ui.toast('ჩანაწერი უკვე შემოწმებულია ან წაშლილია', 'error');
              this.router.navigate(['/console/history']);
              return;
            }
            this.ui.toast('შენახვა ვერ მოხერხდა. სცადეთ თავიდან.', 'error');
          });
        }
      });
  }

}
