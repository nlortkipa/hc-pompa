import { Component, HostListener, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlateService } from '../../../services/plate-service';
import { Plate } from '../../../models/plate-model';
import { UserModel } from '../../../models/user-model';
import { UserService } from '../../../services/user-service';
import { finalize } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Icon } from '../../icon/icon';
import { UiService } from '../../../services/ui.service';
import { SearchableSelect, SearchableSelectOption } from '../../searchable-select/searchable-select';

@Component({
  selector: 'app-plates',
  standalone: true,
  imports: [CommonModule, FormsModule, Icon, SearchableSelect],
  templateUrl: './plates.html',
  styleUrls: ['./plates.scss'],
})
export class Plates implements OnInit {
  plates = signal<Plate[]>([]);
  filteredPlates = signal<Plate[]>([]);
  plateSearch = signal('');
  users = signal<UserModel[]>([]);
  /** Options for the checker picker: users not yet on the plate. Rebuilt on change to keep a stable reference. */
  checkerOptions: SearchableSelectOption[] = [];
  checkerQuery = '';
  
  isLoading = false;
  isSaving = false;
  hasLoaded = false;
  showForm = false;
  plateForm = this.emptyForm();
  isEditing = false;
  errors: Record<string, string> = {};

  constructor(
    private plateService: PlateService,
    private userService: UserService,
    private ui: UiService,
    private cdr: ChangeDetectorRef
  ) {}

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.showForm && !this.ui.dialog()) this.cancelEdit();
  }

  openCreate(): void {
    this.cancelEdit();
    this.showForm = true;
  }

  ngOnInit(): void {
    this.loadPlates();
    this.userService.getAll().subscribe({
      next: (data) => {
        this.users.set([...data].sort((a, b) => this.fullName(a).localeCompare(this.fullName(b), 'ka')));
        this.refreshCheckerOptions();
      },
      error: () => this.ui.toast('მომხმარებლების ჩატვირთვა ვერ მოხერხდა', 'error'),
    });
  }

  private emptyForm() {
    return { id: 0, number: '', type: '', isStationary: false, checkerUserIds: [] as number[] };
  }

  loadPlates(): void {
    this.isLoading = true;
    this.plateService.getPlates().pipe(finalize(() => { this.isLoading = false; this.hasLoaded = true; })).subscribe({
      next: (data) => {
        this.plates.set(data);
        this.filterPlates();
      },
      error: () => {
        this.plates.set([]);
        this.ui.toast('პომპების ჩატვირთვა ვერ მოხერხდა', 'error');
      },
    });
  }

  filterPlates(): void {
    const term = this.plateSearch().trim().toLowerCase();
    this.filteredPlates.set(
      this.plates().filter((p) =>
        p.number.toLowerCase().includes(term) ||
        p.type.toLowerCase().includes(term) ||
        p.checkers.some(c =>
          `${c.firstName} ${c.lastName}`.toLowerCase().includes(term) ||
          (c.email ?? '').toLowerCase().includes(term))
      )
    );
  }

  editPlate(plate: Plate): void {
    this.isEditing = true;
    this.plateForm = {
      id: plate.id,
      number: plate.number,
      type: plate.type,
      isStationary: plate.isStationary,
      checkerUserIds: [...plate.checkerUserIds]
    };
    this.checkerQuery = '';
    this.refreshCheckerOptions();
    this.errors = {};
    this.showForm = true;
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.showForm = false;
    this.plateForm = this.emptyForm();
    this.checkerQuery = '';
    this.refreshCheckerOptions();
    this.errors = {};
  }

  private refreshCheckerOptions(): void {
    const selected = new Set(this.plateForm.checkerUserIds);
    this.checkerOptions = this.users()
      .filter(u => !selected.has(u.id))
      .map(u => ({ value: u.id, label: u.email ? `${this.fullName(u)} · ${u.email}` : this.fullName(u) }));
  }

  onCheckerPicked(userId: number | null): void {
    if (userId == null) return;
    this.addChecker(userId);
    // The select emits the picked label right after the value; clear it once that lands.
    setTimeout(() => this.checkerQuery = '');
  }

  checkerUser(userId: number): UserModel | undefined {
    return this.users().find(u => u.id === userId);
  }

  addChecker(userId: number): void {
    if (!userId || this.plateForm.checkerUserIds.includes(userId)) return;
    this.plateForm.checkerUserIds = [...this.plateForm.checkerUserIds, userId];
    delete this.errors['checkers'];
    this.refreshCheckerOptions();
  }

  removeChecker(userId: number): void {
    this.plateForm.checkerUserIds = this.plateForm.checkerUserIds.filter(id => id !== userId);
    this.refreshCheckerOptions();
  }

  fullName(user: { firstName: string; lastName: string }): string {
    return `${user.firstName} ${user.lastName}`.trim();
  }

  checkerNames(plate: Plate): string {
    return plate.checkers.map(c => this.fullName(c)).join(', ');
  }

  savePlate(): void {
    if (this.isSaving || !this.validate()) return;
    this.isSaving = true;
    const wasEditing = this.isEditing;

    const obs: Observable<any> = this.isEditing
      ? this.plateService.updatePlate(this.plateForm.id, this.plateForm.number, this.plateForm.type, this.plateForm.isStationary, this.plateForm.checkerUserIds)
      : this.plateService.createPlate(this.plateForm.number, this.plateForm.type, this.plateForm.isStationary, this.plateForm.checkerUserIds);

    obs.pipe(finalize(() => (this.isSaving = false))).subscribe({
      next: () => {
        this.loadPlates();
        this.cancelEdit();
        this.ui.toast(wasEditing ? 'პომპა განახლდა' : 'პომპა დაემატა');
      },
      error: (err) => {
        if (err.status === 409) {
          this.errors = { number: 'სანომრე ნიშანი უკვე არსებობს' };
        } else {
          this.ui.toast('შენახვა ვერ მოხერხდა', 'error');
        }
        this.cdr.detectChanges();
      }
    });
  }

  async deletePlate(plate: Plate): Promise<void> {
    const ok = await this.ui.confirm({
      title: `წავშალოთ ${plate.number}?`,
      message: 'სანომრე ნიშანი აღარ გამოჩნდება ახალი ჩანაწერის შექმნისას.',
      confirmLabel: 'წაშლა',
      destructive: true,
    });
    if (!ok) return;
    this.plateService.deletePlate(plate.id).subscribe({
      next: () => {
        this.loadPlates();
        this.ui.toast('პომპა წაიშალა');
      },
      error: () => this.ui.toast('წაშლა ვერ მოხერხდა', 'error'),
    });
  }

  private validate(): boolean {
    this.errors = {};

    if (!this.plateForm.number.trim()) {
      this.errors['number'] = 'სანომრე ნიშანი სავალდებულოა';
    }

    if (!this.plateForm.type.trim()) {
      this.errors['type'] = 'პომპის ტიპი სავალდებულოა';
    }

    if (this.plateForm.checkerUserIds.length === 0) {
      this.errors['checkers'] = 'მინიმუმ ერთი შემმოწმებელი სავალდებულოა';
    }

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }
}
