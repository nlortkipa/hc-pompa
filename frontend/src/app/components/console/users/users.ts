import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user-service';
import { UserModel } from '../../../models/user-model';
import { finalize } from 'rxjs';

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
        error: (err) => console.error('Failed to load users:', err)
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
    if (this.isLoading) return;
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
        console.error('Failed to save user:', err);
        alert('შენახვა ვერ მოხერხდა. შესაძლოა ნომერი უკვე გამოყენებულია.');
      }
    });
  }

  deleteUser(id: number): void {
    if (!confirm('ნამდვილად გსურთ მომხმარებლის წაშლა?')) return;
    
    this.isLoading = true;
    this.userService.delete(id)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: () => this.loadUsers(),
        error: (err) => console.error('Failed to delete user:', err)
      });
  }
}
