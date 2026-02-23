import { Injectable } from '@angular/core';
import Keycloak       from 'keycloak-js';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class KeycloakService {
  private _keycloak: Keycloak | null = null;

  constructor() {
    this._keycloak = new Keycloak({
      url:   environment.keycloak.url,
      realm: environment.keycloak.realm,
      clientId: environment.keycloak.clientId
    });
  }

  private get keycloak(): Keycloak {
    if (!this._keycloak) {
      this._keycloak = new Keycloak({
        url:      environment.keycloak.url,
        realm:    environment.keycloak.realm,
        clientId: environment.keycloak.clientId
      });
    }
    return this._keycloak;
  }

  // Chamado pelo APP_INITIALIZER antes do bootstrap
  async init(): Promise<boolean> {
    try {
      return await this.keycloak.init({
        onLoad:           'login-required',
        checkLoginIframe: false,
        pkceMethod:       'S256'   // PKCE — padrão seguro para SPAs
      });
    } catch (err) {
      console.error('[Keycloak] Falha na inicialização:', err);
      return false;
    }
  }

  get token(): string | undefined {
    return this.keycloak.token;
  }

  get isLoggedIn(): boolean {
    return !!this.keycloak.authenticated;
  }

  async logout(): Promise<void> {
    await this.keycloak.logout({
      redirectUri: window.location.origin
    });
  }

  // Renova o token se expirar em menos de 30s
  async refreshToken(): Promise<boolean> {
    try {
      return await this.keycloak.updateToken(30);
    } catch {
      await this.logout();
      return false;
    }
  }

  getUserName(): string {
    return this.keycloak.tokenParsed?.['preferred_username'] ?? '';
  }
  
  getFullName(): string {
    const parsed = this.keycloak.tokenParsed;
    return parsed?.['name']
      ?? `${parsed?.['given_name'] ?? ''} ${parsed?.['family_name'] ?? ''}`.trim()
      ?? this.getUserName();
  }
}
