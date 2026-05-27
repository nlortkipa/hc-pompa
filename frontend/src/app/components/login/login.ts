import { Component } from '@angular/core';
import { APP_NAME } from '../../globals';
import { RouterLink } from "@angular/router";

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  appName = APP_NAME
}
