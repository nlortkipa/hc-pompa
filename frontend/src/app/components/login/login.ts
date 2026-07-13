import { Component, signal } from '@angular/core';
import { APP_NAME } from '../../globals';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth-service';
import { switchMap } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  appName = APP_NAME;

  number = signal('');
  error = signal('');
  showError = signal(false);
  isLoading = signal(false);

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit() {
    const token = localStorage.getItem('token')
    if (token != null)
      this.router.navigate(['/console'])
  }

  submit() {
    if (!this.number().trim()) {
      this.error.set('შეიყვანეთ ტელეფონის ნომერი');
      this.showError.set(true);
      return;
    }

    const cleanNumber = this.number().replace(/\s/g, '');
    this.isLoading.set(true);

    this.authService.sendCode(cleanNumber).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/verify']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.error.set('კოდის გაგზავნა ვერ მოხერხდა.');
        this.showError.set(true);
      }
    });
  }
}
