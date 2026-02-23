//Com APP_INITIALIZER, o guard não precisa mais chamar keycloak.init() — isso já aconteceu antes do Angular renderizar qualquer rota.
import { inject }          from '@angular/core';
import { CanActivateFn }   from '@angular/router';
import { KeycloakService } from './keycloak.service';

// Guard funcional — padrão Angular
// O Keycloak já foi inicializado pelo APP_INITIALIZER,
// então aqui só verificamos se o usuário está autenticado.
export const authGuard: CanActivateFn = () => {
  const keycloak = inject(KeycloakService);
  return keycloak.isLoggedIn;
};
