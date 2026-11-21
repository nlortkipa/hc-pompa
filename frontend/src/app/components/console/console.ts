import { Component, HostListener, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileService } from '../../services/profile-service';
import { Router, RouterOutlet, RouterLinkActive, RouterLinkWithHref } from '@angular/router';
import { APP_NAME } from '../../globals';
import { UserModel } from '../../models/user-model';
import { Icon } from '../icon/icon';
import { UiService } from '../../services/ui.service';
import { ReviewInboxService } from '../../services/review-inbox.service';

interface NavItem {
  path: string;
  label: string;
  short: string;
  icon: string;
  roles?: string[];
  hideFor?: string[];
  /** Only shown to users who have been assigned review requests. */
  checkersOnly?: boolean;
}

const NAV: NavItem[] = [
  { path: 'new', label: 'ახალი ჩანაწერი', short: 'ახალი', icon: 'plus-circle', hideFor: ['Accountant'] },
  { path: 'history', label: 'რეპორტები', short: 'რეპორტები', icon: 'doc' },
  { path: 'reviews', label: 'შემოწმება', short: 'შემოწმება', icon: 'check-circle', checkersOnly: true },
  { path: 'users', label: 'მომხმარებლები', short: 'მომხმ.', icon: 'people', roles: ['Admin'] },
  { path: 'companies', label: 'კომპანიები', short: 'კომპანიები', icon: 'building', roles: ['Admin'] },
  { path: 'plates', label: 'პომპები', short: 'პომპები', icon: 'truck', roles: ['Admin'] },
];

@Component({
  standalone: true,
  selector: 'app-console',
  imports: [CommonModule, RouterOutlet, RouterLinkActive, RouterLinkWithHref, Icon],
  templateUrl: './console.html',
  styleUrl: './console.scss',
})
export class Console {
  appName = APP_NAME;
  private router = inject(Router);
  private profileService = inject(ProfileService);
  private ui = inject(UiService);
  private reviewInbox = inject(ReviewInboxService);
  profile = this.profileService.profile;
  pendingReviews = this.reviewInbox.pendingCount;

  readonly isMac = typeof navigator !== 'undefined' && /Mac|iPhone|iPad/.test(navigator.platform);
  readonly modKey = this.isMac ? '⌥' : 'Alt+';

  navItems = computed(() => {
    const role = this.profile()?.role;
    const isChecker = this.reviewInbox.reviews().length > 0;
    const visible = NAV.filter(item => !item.checkersOnly || isChecker);
    if (!role) return visible.filter(item => !item.roles);
    return visible.filter(item =>
      (!item.roles || item.roles.includes(role)) && !(item.hideFor?.includes(role))
    );
  });

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token == null) {
      this.router.navigate(['/login']);
      return;
    }

    this.profileService.getProfile(token).subscribe({
      next: () => this.loadReviewInbox(),
      error: () => this.signOut(),
    });
  }

  /** Checkers see their pending reports right after signing in. */
  private loadReviewInbox() {
    this.reviewInbox.refresh().subscribe({
      next: () => {
        const pending = this.reviewInbox.pendingCount();
        if (pending > 0 && !this.router.url.startsWith('/console/reviews')) {
          this.ui.toast(`${pending} რეპორტი გელოდებათ შესამოწმებლად`, 'info', 5000);
        }
      },
      error: () => {},
    });
  }

  /** Alt/⌥ + 1…5 jumps between sections. */
  @HostListener('document:keydown', ['$event'])
  onKeydown(event: KeyboardEvent): void {
    if (!event.altKey || event.ctrlKey || event.metaKey) return;
    const index = Number(event.code.replace('Digit', '')) - 1;
    const item = this.navItems()[index];
    if (!item) return;
    event.preventDefault();
    this.router.navigate(['/console', item.path]);
  }

  async logout() {
    const ok = await this.ui.confirm({
      title: 'გსურთ გასვლა?',
      message: 'ხელახლა შესასვლელად დაგჭირდებათ სმს კოდი.',
      confirmLabel: 'გასვლა',
      destructive: true,
    });
    if (ok) this.signOut();
  }

  private signOut() {
    localStorage.removeItem('token');
    this.profileService.clearProfile();
    this.reviewInbox.clear();
    this.router.navigate(['/login']);
  }

  displayName(user: UserModel | null): string {
    if (!user) return '';
    return user.username?.trim() || `${user.firstName ?? ''} ${user.lastName ?? ''}`.trim();
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
