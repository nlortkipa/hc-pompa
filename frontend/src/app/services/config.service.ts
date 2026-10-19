import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConfigService {
  private config: any;

  constructor(private http: HttpClient) {}

  loadConfig() {
    return firstValueFrom(this.http.get<any>('/config.json'))
      .then(config => {
        this.config = config;
      });
  }

  get backendUrl() {
    const configuredBackendUrl = this.config?.BACKEND_URL;
    return typeof configuredBackendUrl === 'string'
      ? configuredBackendUrl
      : 'https://localhost:7067';
  }
}
