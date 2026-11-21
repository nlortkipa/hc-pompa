import { Component, HostListener, OnInit, signal, computed, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user-service';
import { ProfileService } from '../../../services/profile-service';
import { UserModel } from '../../../models/user-model';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';
import { Icon } from '../../icon/icon';
import { UiService } from '../../../services/ui.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, Icon],
  templateUrl: './users.html',
  styleUrls: ['./users.scss'],
})
export class Users implements OnInit {
  users = signal<UserModel[]>([]);
  isLoading = false;
  isEditing = false;
  isSaving = false;
  showForm = false;
  hasLoaded = false;
  search = signal('');
  roleFilter = signal<'all' | 'User' | 'Accountant' | 'Admin'>('all');

  filteredUsers = computed(() => {
    const term = this.search().trim().toLowerCase();
    const role = this.roleFilter();
    return this.users().filter(u =>
      (role === 'all' || u.role === role) &&
      (!term ||
        `${u.firstName} ${u.lastName}`.toLowerCase().includes(term) ||
        u.phoneNumber?.includes(term) ||
        u.email?.toLowerCase().includes(term) ||
        u.personalId?.includes(term))
    );
  });

  countFor(role: string): number {
    return this.users().filter(u => u.role === role).length;
  }
  
  userForm: any = this.defaultForm();
  errors: Record<string, string> = {};

  constructor(
    private userService: UserService,
    private profileService: ProfileService,
    private router: Router,
    private ui: UiService,
    private cdr: ChangeDetectorRef
  ) {}

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.showForm && !this.ui.dialog()) this.cancelEdit();
  }

  openCreate(): void {
    this.isEditing = false;
    this.userForm = this.defaultForm();
    this.errors = {};
    this.showForm = true;
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  defaultForm(): any {
    return {
      id: 0,
      firstName: '',
      lastName: '',
      personalId: '',
      phoneNumber: '',
      email: '',
      role: 'User'
    };
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAll()
      .pipe(finalize(() => {
        this.isLoading = false;
        this.hasLoaded = true;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (data) => this.users.set(data),
        error: (err) => {
          console.error('მომხმარებლების ჩატვირთვა ვერ მოხერხდა:', err);
          this.ui.toast('მომხმარებლების ჩატვირთვა ვერ მოხერხდა', 'error');
        }
      });
  }

  editUser(user: UserModel): void {
    this.isEditing = true;
    this.userForm = { ...user, email: user.email ?? '' };
    this.errors = {};
    this.showForm = true;
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.showForm = false;
    this.userForm = this.defaultForm();
    this.errors = {};
  }

  validate(): boolean {
    this.errors = {};
    if (!this.userForm.firstName?.trim()) this.errors['firstName'] = 'სახელი სავალდებულოა';
    if (!this.userForm.lastName?.trim()) this.errors['lastName'] = 'გვარი სავალდებულოა';
    if (!this.userForm.phoneNumber?.trim()) this.errors['phoneNumber'] = 'ტელეფონი სავალდებულოა';
    
    if (!this.userForm.personalId?.trim()) {
      this.errors['personalId'] = 'პირადი ნომერი სავალდებულოა';
    } else if (this.userForm.personalId.trim().length !== 11) {
      this.errors['personalId'] = 'პირადი ნომერი უნდა იყოს 11 ნიშნა';
    }

    const email = (this.userForm.email ?? '').trim();
    if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.errors['email'] = 'შეიყვანეთ სწორი ელფოსტა';
    }

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }

  saveUser(): void {
    this.userForm.phoneNumber = (this.userForm.phoneNumber ?? '').replace(/\s+/g, '');
    this.userForm.email = (this.userForm.email ?? '').trim().toLowerCase() || null;
    if (this.isSaving) return;
    if (this.isEditingOwnLastAdmin() && this.userForm.role !== 'Admin') {
      this.userForm.role = 'Admin';
      this.ui.alert('როლის შეცვლა შეუძლებელია', 'საკუთარ როლს ვერ შეცვლით, რადგან ერთადერთი ადმინისტრატორი ხართ.');
      return;
    }
    if (!this.validate()) return;
    this.isSaving = true;
    const wasEditing = this.isEditing;

    const obs = this.isEditing 
      ? this.userService.update(this.userForm.id, this.userForm)
      : this.userService.create(this.userForm);

    obs.pipe(finalize(() => {
      this.isSaving = false;
      this.cdr.detectChanges();
    }))
    .subscribe({
      next: () => {
        this.cancelEdit();
        this.loadUsers();
        this.ui.toast(wasEditing ? 'მომხმარებელი განახლდა' : 'მომხმარებელი დაემატა');
      },
      error: (err) => {
        if (err.status === 409) {
          this.ui.alert('მომხმარებელი უკვე არსებობს', 'მომხმარებელი მსგავსი მონაცემებით უკვე არსებობს.');
        } else {
          console.error('მომხმარებლის შენახვა ვერ მოხერხდა:', err);
          this.ui.toast('შენახვა ვერ მოხერხდა', 'error');
        }
      }
    });
  }

  sendingLinkId: number | null = null;

  async sendLink(user: UserModel): Promise<void> {
    const ok = await this.ui.confirm({
      title: 'გავუგზავნოთ ბმული?',
      message: `საიტის ბმული გაეგზავნება ${user.firstName} ${user.lastName}-ს ნომერზე ${user.phoneNumber}.`,
      confirmLabel: 'გაგზავნა',
    });
    if (!ok) return;

    this.sendingLinkId = user.id;
    this.userService.sendLink(user.id)
      .pipe(finalize(() => {
        this.sendingLinkId = null;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: () => this.ui.toast('ბმული გაიგზავნა'),
        error: (err) => {
          console.error('ბმულის გაგზავნა ვერ მოხერხდა:', err);
          this.ui.toast('ბმულის გაგზავნა ვერ მოხერხდა', 'error');
        }
      });
  }

  async deleteUser(user: UserModel): Promise<void> {
    const isSelf = this.isCurrentUser(user);
    if (this.isOnlyAdmin(user)) {
        this.ui.alert('წაშლა შეუძლებელია', isSelf
          ? 'საკუთარ თავს ვერ წაშლით, რადგან ერთადერთი ადმინისტრატორი ხართ.'
          : 'ერთადერთი ადმინისტრატორის წაშლა შეუძლებელია.');
        return;
    }

    if (isSelf) {
      const ok = await this.ui.confirm({
        title: 'წავშალოთ თქვენი ანგარიში?',
        message: 'წაშლის შემდეგ ავტომატურად გამოხვალთ სისტემიდან.',
        confirmLabel: 'წაშლა',
        destructive: true,
      });
      if (!ok) return;

      this.isLoading = true;
      this.userService.delete(user.id)
        .pipe(finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }))
        .subscribe({
          next: () => {
            localStorage.removeItem('token');
            this.router.navigate(['/login']);
          },
          error: (err) => {
            if (err.status === 409) {
              this.ui.alert('მოქმედება ვერ შესრულდა', 'ბოლო ადმინისტრატორი წაიშლებოდა.');
            } else {
              console.error('მომხმარებლის წაშლა ვერ მოხერხდა:', err);
              this.ui.toast('წაშლა ვერ მოხერხდა', 'error');
            }
          }
        });
      return;
    }

    const profile = this.profileService.getProfileData();
    if (user.id === profile?.id) {
        this.ui.alert('წაშლა შეუძლებელია', 'საკუთარი თავის წაშლა არ შეიძლება.');
        return;
    }
    if (user.role === 'Admin') {
        this.ui.alert('წაშლა შეუძლებელია', 'ადმინისტრატორის წაშლა არ შეიძლება.');
        return;
    }
    const ok = await this.ui.confirm({
      title: `წავშალოთ ${user.firstName} ${user.lastName}?`,
      message: 'მომხმარებელი ვეღარ შეძლებს სისტემაში შესვლას.',
      confirmLabel: 'წაშლა',
      destructive: true,
    });
    if (!ok) return;
    
    this.isLoading = true;
    this.userService.delete(user.id)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: () => {
          this.loadUsers();
          this.ui.toast('მომხმარებელი წაიშალა');
        },
        error: (err) => {
          console.error('მომხმარებლის წაშლა ვერ მოხერხდა:', err);
          this.ui.toast('წაშლა ვერ მოხერხდა', 'error');
        }
      });
  }

  roleClass(role?: string | null): string {
    if (role === 'Admin') return 'badge-purple';
    if (role === 'Accountant') return 'badge-yellow';
    return 'badge-blue';
  }

  initials(u: UserModel): string {
    return `${u.firstName?.charAt(0) ?? ''}${u.lastName?.charAt(0) ?? ''}`;
  }

  roleLabel(role?: string | null): string {
    if (role === 'Admin') return 'ადმინისტრატორი';
    if (role === 'Accountant') return 'ბუღალტერი';
    if (role === 'User') return 'ოპერატორი';
    return role ?? '';
  }

  isEditingOwnLastAdmin(): boolean {
    const profile = this.profileService.getProfileData();
    return this.isEditing &&
      this.userForm.id === profile?.id &&
      profile?.role === 'Admin' &&
      this.adminCount() <= 1;
  }

  private isCurrentUser(user: UserModel): boolean {
    return user.id === this.profileService.getProfileData()?.id;
  }

  private isOnlyAdmin(user: UserModel): boolean {
    return user.role === 'Admin' && this.adminCount() <= 1;
  }

  private adminCount(): number {
    return this.users().filter(user => user.role === 'Admin').length;
  }
}
