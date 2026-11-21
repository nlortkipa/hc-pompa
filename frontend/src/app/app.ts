import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BackendAvailabilityService } from './services/backend-availability.service';
import { UiOverlay } from './components/ui-overlay/ui-overlay';
import { Icon } from './components/icon/icon';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, UiOverlay, Icon],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  constructor(public backendAvailability: BackendAvailabilityService) {}
}
