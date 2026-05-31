import { Component, signal } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { Router, RouterOutlet, RouterLinkActive, RouterLinkWithHref } from '@angular/router';
import { APP_NAME } from '../../globals';
import { UserModel } from '../../models/user-model';
import { New } from "./new/new";

@Component({
  standalone: true,
  selector: 'app-console',
  imports: [New, RouterOutlet, RouterLinkActive, RouterLinkWithHref],
  templateUrl: './console.html',
  styleUrl: './console.scss',
})
export class Console {
  appName = APP_NAME
  profile = signal<UserModel | null>(null)

  constructor(private router: Router, private profileService: ProfileService) { }

  ngOnInit() {
    const token = localStorage.getItem('token')
    if (token == null) {
      this.router.navigate(['/login'])
      return
    }

    this.profileService.getProfile(token).subscribe({
      next: (data) => this.profile.set(data),
      error: () => {
        localStorage.removeItem('token')
        this.router.navigate(['/login'])
      }
    })
  }
}
