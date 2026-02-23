import {
  ApplicationConfig,
  APP_INITIALIZER,
  provideZoneChangeDetection
} from '@angular/core';
import { provideRouter }           from '@angular/router';
import { provideAnimationsAsync }  from '@angular/platform-browser/animations/async';
import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';
import { routes }                   from './app.routes';
import { apiInterceptor }           from './core/http/api.interceptor';
import { correlationIdInterceptor } from './core/http/correlation-id.interceptor';
import { KeycloakService }          from './core/auth/keycloak.service';

// Inicializa o Keycloak antes do Angular renderizar qualquer componente.
// O APP_INITIALIZER garante que o token já está disponível nos interceptors.
function initKeycloak(keycloak: KeycloakService): () => Promise<boolean> {
  return () => keycloak.init();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([correlationIdInterceptor, apiInterceptor])
    ),
    {
      provide:    APP_INITIALIZER,
      useFactory: initKeycloak,
      deps:       [KeycloakService],
      multi:      true
    }
  ]
};

