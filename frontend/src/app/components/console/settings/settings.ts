import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { UserModel } from '../../../models/user-model';
import { ProfileService } from '../../../services/profile-service';

type SettingsTab = 'account' | 'security' | 'notifications' | 'data';
type NotificationKey = 'reviews' | 'summaries' | 'security';

const SETTINGS_PREFS_KEY = 'settings_notification_preferences';
const MAX_AVATAR_BYTES = 1_500_000;

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings implements OnInit {
  private profileService = inject(ProfileService);
  private router = inject(Router);

  readonly profile = this.profileService.profile;
  readonly activeTab = signal<SettingsTab>('account');
  readonly isSaving = signal(false);
  readonly isActionRunning = signal(false);
  readonly isDragging = signal(false);
  readonly message = signal('');
  readonly error = signal('');

  readonly tabs: { id: SettingsTab; label: string; icon: string }[] = [
    { id: 'account', label: 'ანგარიში', icon: '@' },
    { id: 'security', label: 'უსაფრთხოება', icon: '#' },
    { id: 'notifications', label: 'შეტყობინებები', icon: '*' },
    { id: 'data', label: 'მონაცემები', icon: '!' },
  ];

  accountForm = {
    username: '',
    avatarUrl: null as string | null,
  };

  notificationPrefs: Record<NotificationKey, boolean> = {
    reviews: true,
    summaries: true,
    security: true,
  };

  constructor() {}

  ngOnInit(): void {
    this.loadNotificationPrefs();

    const currentProfile = this.profile();
    if (currentProfile) {
      this.syncForm(currentProfile);
      return;
    }

    const token = localStorage.getItem('token');
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    this.profileService.getProfile(token).subscribe({
      next: (profile) => this.syncForm(profile),
      error: () => this.router.navigate(['/login']),
    });
  }

  openTab(tab: SettingsTab): void {
    this.activeTab.set(tab);
    this.message.set('');
    this.error.set('');
  }

  saveAccount(): void {
    const username = this.accountForm.username.trim();
    if (!username) {
      this.error.set('მომხმარებლის სახელი სავალდებულოა');
      return;
    }

    if (this.isSaving()) return;

    this.isSaving.set(true);
    this.message.set('');
    this.error.set('');

    this.profileService.updateAccount({
      username,
      avatarUrl: this.accountForm.avatarUrl,
    }).pipe(
      finalize(() => this.isSaving.set(false))
    ).subscribe({
      next: (profile) => {
        this.syncForm(profile);
        this.message.set('ანგარიში განახლდა');
      },
      error: (err) => {
        this.error.set(err?.error || 'ანგარიშის განახლება ვერ მოხერხდა');
      },
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) this.loadAvatarFile(file);
    input.value = '';
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(true);
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(false);
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(false);

    const file = event.dataTransfer?.files?.[0];
    if (file) this.loadAvatarFile(file);
  }

  removeAvatar(): void {
    this.accountForm.avatarUrl = null;
    this.message.set('');
    this.error.set('');
  }

  logoutAllDevices(): void {
    if (this.isActionRunning()) return;
    if (!confirm('გამოვიდეთ ყველა მოწყობილობიდან?')) return;

    this.isActionRunning.set(true);
    this.profileService.logoutAllDevices()
      .pipe(finalize(() => this.isActionRunning.set(false)))
      .subscribe({
        next: () => this.finishSession(),
        error: (err) => this.error.set(err?.error || 'მოწყობილობებიდან გასვლა ვერ მოხერხდა'),
      });
  }

  deleteAccount(): void {
    if (this.isActionRunning()) return;
    const confirmed = confirm('ნამდვილად გსურთ ანგარიშის წაშლა? ეს მოქმედება შეუქცევადია.');
    if (!confirmed) return;

    this.isActionRunning.set(true);
    this.profileService.deleteAccount()
      .pipe(finalize(() => this.isActionRunning.set(false)))
      .subscribe({
        next: () => this.finishSession(),
        error: (err) => {
          if (err.status === 409) {
            this.error.set('ანგარიშის წაშლა შეუძლებელია, რადგან ბოლო ადმინისტრატორი ხართ');
            return;
          }

          this.error.set(err?.error || 'ანგარიშის წაშლა ვერ მოხერხდა');
        },
      });
  }

  setNotificationPref(key: NotificationKey, value: boolean): void {
    this.notificationPrefs = {
      ...this.notificationPrefs,
      [key]: value,
    };
    localStorage.setItem(SETTINGS_PREFS_KEY, JSON.stringify(this.notificationPrefs));
  }

  displayName(user: UserModel | null): string {
    if (!user) return '';
    return user.username?.trim() || `${user.firstName} ${user.lastName}`.trim();
  }

  initials(user: UserModel | null): string {
    if (!user) return '';
    const first = user.firstName?.trim().charAt(0) ?? '';
    const last = user.lastName?.trim().charAt(0) ?? '';
    return `${first}${last}` || this.displayName(user).charAt(0);
  }

  roleLabel(role?: string | null): string {
    if (role === 'Admin') return 'ადმინისტრატორი';
    if (role === 'Accountant') return 'ბუღალტერი';
    if (role === 'User') return 'ოპერატორი';
    return role ?? '';
  }

  private syncForm(profile: UserModel): void {
    this.accountForm = {
      username: this.displayName(profile),
      avatarUrl: profile.avatarUrl ?? null,
    };
  }

  private loadAvatarFile(file: File): void {
    this.message.set('');
    this.error.set('');

    if (!file.type.startsWith('image/')) {
      this.error.set('აირჩიეთ სურათის ფაილი');
      return;
    }

    if (file.size > MAX_AVATAR_BYTES) {
      this.error.set('სურათი ძალიან დიდია');
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.accountForm.avatarUrl = reader.result as string;
    };
    reader.onerror = () => this.error.set('სურათის წაკითხვა ვერ მოხერხდა');
    reader.readAsDataURL(file);
  }

  private loadNotificationPrefs(): void {
    try {
      const raw = localStorage.getItem(SETTINGS_PREFS_KEY);
      if (raw) {
        this.notificationPrefs = {
          ...this.notificationPrefs,
          ...JSON.parse(raw),
        };
      }
    } catch {
      localStorage.removeItem(SETTINGS_PREFS_KEY);
    }
  }

  private finishSession(): void {
    localStorage.removeItem('token');
    this.profileService.clearProfile();
    this.router.navigate(['/login']);
  }
}
