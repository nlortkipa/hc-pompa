import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlateService } from '../../../services/plate-service';
import { Plate } from '../../../models/plate-model';
import { finalize } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-plates',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './plates.html',
  styleUrls: ['./plates.scss'],
})
export class Plates implements OnInit {
  plates = signal<Plate[]>([]);
  filteredPlates = signal<Plate[]>([]);
  plateSearch = signal('');
  
  isLoading = false;
  plateForm = { id: 0, number: '', checkerGmail: '' };
  isEditing = false;
  errors: Record<string, string> = {};

  constructor(
    private plateService: PlateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadPlates();
  }

  loadPlates(): void {
    this.isLoading = true;
    this.plateService.getPlates().pipe(finalize(() => this.isLoading = false)).subscribe({
      next: (data) => {
        this.plates.set(data);
        this.filterPlates();
      },
      error: () => this.plates.set([]),
    });
  }

  filterPlates(): void {
    const term = this.plateSearch().toLowerCase();
    this.filteredPlates.set(
      this.plates().filter((p) => p.number.toLowerCase().includes(term))
    );
  }

  editPlate(plate: Plate): void {
    this.isEditing = true;
    this.plateForm = { ...plate };
    this.filterPlates();
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.plateForm = { id: 0, number: '', checkerGmail: '' };
    this.errors = {};
  }

  savePlate(): void {
    if (!this.validate()) return;
    this.isLoading = true;

    const obs: Observable<any> = this.isEditing
      ? this.plateService.updatePlate(this.plateForm.id, this.plateForm.number, this.plateForm.checkerGmail)
      : this.plateService.createPlate(this.plateForm.number, this.plateForm.checkerGmail);

    obs.pipe(finalize(() => (this.isLoading = false))).subscribe({
      next: () => {
        this.loadPlates();
        this.cancelEdit();
      },
      error: (err) => {
        if (err.status === 409) {
          alert('სანომრე ნიშანი უკვე არსებობს.');
        } else {
          alert('შენახვა ვერ მოხერხდა.');
        }
      }
    });
  }

  deletePlate(id: number): void {
    if (!confirm('ნამდვილად გსურთ სანომრე ნიშნის წაშლა?')) return;
    this.isLoading = true;
    this.plateService.deletePlate(id).pipe(finalize(() => (this.isLoading = false))).subscribe({
      next: () => {
        this.loadPlates();
      }
    });
  }

  private validate(): boolean {
    this.errors = {};

    if (!this.plateForm.number.trim()) {
      this.errors['number'] = 'სანომრე ნიშანი სავალდებულოა';
    }

    const checkerGmail = this.plateForm.checkerGmail.trim();
    if (!checkerGmail) {
      this.errors['checkerGmail'] = 'შემმოწმებლის ელფოსტა სავალდებულოა';
    } else if (!checkerGmail.includes('@') || !checkerGmail.includes('.')) {
      this.errors['checkerGmail'] = 'შეიყვანეთ სწორი ელფოსტა';
    }

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }
}
