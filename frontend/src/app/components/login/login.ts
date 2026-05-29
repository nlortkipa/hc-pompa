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

  constructor(private router: Router, private authService: AuthService) { }

  submit() {
    if (!this.number().trim()) {
      this.error.set('შეიყვანეთ ტელეფონის ნომერი');
      this.showError.set(true);
      return;
    }

    const cleanNumber = this.number().replace(/\s/g, '');

    this.authService.sendCode(cleanNumber).subscribe({
      next: () => this.router.navigate(['/verify']),
      error: (err) => {
        this.error.set(err.error);
        this.showError.set(true);
      }
    });
  }
}