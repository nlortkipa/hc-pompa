import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user-service';
import { ProfileService } from '../../../services/profile-service';
import { UserModel } from '../../../models/user-model';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.html',
  styleUrls: ['./users.scss'],
})
export class Users implements OnInit {
  users = signal<UserModel[]>([]);
  isLoading = false;
  isEditing = false;
  
  userForm: any = this.defaultForm();
  errors: Record<string, string> = {};

  constructor(
    private userService: UserService,
    private profileService: ProfileService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

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
      role: 'User'
    };
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAll()
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (data) => this.users.set(data),
        error: (err) => console.error('მომხმარებლების ჩატვირთვა ვერ მოხერხდა:', err)
      });
  }

  editUser(user: UserModel): void {
    this.isEditing = true;
    this.userForm = { ...user };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.isEditing = false;
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

    this.cdr.detectChanges();
    return Object.keys(this.errors).length === 0;
  }

  saveUser(): void {
    this.userForm.phoneNumber = this.userForm.phoneNumber.replace(/\s+/g, '');
    if (this.isLoading) return;
    if (this.isEditingOwnLastAdmin() && this.userForm.role !== 'Admin') {
      this.userForm.role = 'Admin';
      alert('საკუთარ როლს ვერ შეცვლით, რადგან ერთადერთი ადმინისტრატორი ხართ.');
      return;
    }
    if (!this.validate()) return;
    this.isLoading = true;

    const obs = this.isEditing 
      ? this.userService.update(this.userForm.id, this.userForm)
      : this.userService.create(this.userForm);

    obs.pipe(finalize(() => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }))
    .subscribe({
      next: () => {
        this.cancelEdit();
        this.loadUsers();
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 409) {
          alert('მომხმარებელი მსგავსი მონაცემებით უკვე არსებობს.');
        } else {
          console.error('მომხმარებლის შენახვა ვერ მოხერხდა:', err);
          alert('შენახვა ვერ მოხერხდა.');
        }
      }
    });
  }

  deleteUser(user: UserModel): void {
    const isSelf = this.isCurrentUser(user);
    if (this.isOnlyAdmin(user)) {
        alert(isSelf
          ? 'საკუთარ თავს ვერ წაშლით, რადგან ერთადერთი ადმინისტრატორი ხართ.'
          : 'ერთადერთი ადმინისტრატორის წაშლა შეუძლებელია.');
        return;
    }

    if (isSelf) {
      if (!confirm('ნამდვილად გსურთ საკუთარი ანგარიშის წაშლა?')) return;

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
              alert('მოქმედება ვერ შესრულდა, რადგან ბოლო ადმინისტრატორი წაიშლებოდა.');
            } else {
              console.error('მომხმარებლის წაშლა ვერ მოხერხდა:', err);
            }
          }
        });
      return;
    }

    const profile = this.profileService.getProfileData();
    if (user.id === profile?.id) {
        alert('საკუთარი თავის წაშლა არ შეიძლება.');
        return;
    }
    if (user.role === 'Admin') {
        alert('ადმინისტრატორის წაშლა არ შეიძლება.');
        return;
    }
    if (!confirm('ნამდვილად გსურთ მომხმარებლის წაშლა?')) return;
    
    this.isLoading = true;
    this.userService.delete(user.id)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: () => this.loadUsers(),
        error: (err) => console.error('მომხმარებლის წაშლა ვერ მოხერხდა:', err)
      });
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
