import { Injectable } from '@angular/core';
import Keycloak       from 'keycloak-js';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class KeycloakService {
  private _keycloak: Keycloak;

  constructor() {
    this._keycloak = new Keycloak({
      url:   environment.keycloak.url,
      realm: environment.keycloak.realm,
      clientId: environment.keycloak.clientId
    });
  }

  async init(): Promise<boolean> {
    return this._keycloak.init({
      onLoad:         'login-required',
      checkLoginIframe: false
    });
  }

  get token(): string | undefined {
    return this._keycloak.token;
  }

  get isLoggedIn(): boolean {
    return !!this._keycloak.authenticated;
  }

  async logout(): Promise<void> {
    await this._keycloak.logout({
      redirectUri: window.location.origin
    });
  }

  async refreshToken(): Promise<boolean> {
    return this._keycloak.updateToken(30);
  }

  getUserName(): string {
    return this._keycloak.tokenParsed?.['preferred_username'] ?? '';
  }
}
