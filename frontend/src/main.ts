//Com APP_INITIALIZER, o main.ts fica limpo — o Keycloak é inicializado automaticamente pelo Angular antes do primeiro componente renderizar.
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig }            from './app/app.config';
import { AppComponent }         from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch(err => console.error('[Bootstrap] Erro:', err));
