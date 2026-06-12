import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PumpOperationService } from '../../../services/pump-operation-service';
import { CompanyService } from '../../../services/company-service';
import { UserService } from '../../../services/user-service';
import { ProfileService } from '../../../services/profile-service';
import { PumpOperationModel } from '../../../models/pump-operation';
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
  records = signal<any[]>([]);
  isLoading = false;
  
  isAdmin = false;
  users = signal<UserModel[]>([]);

  filter = {
    fromDate: '',
    toDate: '',
    companyId: null as number | null,
    operatorId: null as number | null
  };

  constructor(
    private pumpOperationService: PumpOperationService,
    public companyService: CompanyService,
    private userService: UserService,
    private profileService: ProfileService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const profile = this.profileService.getProfileData();
    this.isAdmin = profile?.role === 'Admin';
    
    if (this.isAdmin) {
      this.userService.getAll().subscribe(data => this.users.set(data));
    }
    
    this.loadRecords();
  }

  loadRecords(): void {
    this.isLoading = true;
    const f: any = {};
    if (this.filter.fromDate) f.fromDate = this.filter.fromDate;
    if (this.filter.toDate) f.toDate = this.filter.toDate;
    if (this.filter.companyId) f.companyId = this.filter.companyId;
    if (this.filter.operatorId) f.operatorId = this.filter.operatorId;

    this.pumpOperationService.getAll(f)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (data) => this.records.set(data),
        error: (err) => console.error('Failed to load records:', err)
      });
  }

  clearFilters(): void {
    this.filter = {
      fromDate: '',
      toDate: '',
      companyId: null,
      operatorId: null
    };
    this.loadRecords();
  }
}
