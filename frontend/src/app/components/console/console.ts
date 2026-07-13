import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileService } from '../../services/profile-service';
import { Router, RouterOutlet, RouterLinkActive, RouterLinkWithHref } from '@angular/router';
import { APP_NAME } from '../../globals';
import { UserModel } from '../../models/user-model';

@Component({
  standalone: true,
  selector: 'app-console',
  imports: [CommonModule, RouterOutlet, RouterLinkActive, RouterLinkWithHref],
  templateUrl: './console.html',
  styleUrl: './console.scss',
})
export class Console {
  appName = APP_NAME
  private router = inject(Router)
  private profileService = inject(ProfileService)
  profile = this.profileService.profile

  constructor() { }

  ngOnInit() {
    const token = localStorage.getItem('token')
    if (token == null) {
      this.router.navigate(['/login'])
      return
    }

    this.profileService.getProfile(token).subscribe({
      next: () => {},
      error: () => {
        this.logout();
      }
    })
  }

  logout() {
    localStorage.removeItem('token');
    this.profileService.clearProfile();
    this.router.navigate(['/login']);
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
}
