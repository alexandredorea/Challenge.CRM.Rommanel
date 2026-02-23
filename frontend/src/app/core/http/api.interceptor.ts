import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject }                               from '@angular/core';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { KeycloakService }                      from '../auth/keycloak.service';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const keycloak = inject(KeycloakService);

  // Não intercepta chamadas externas (ex: ViaCEP direto)
  if (!req.url.startsWith('/api/')) return next(req);

  return from(keycloak.refreshToken()).pipe(
    switchMap(() => {
      const token  = keycloak.token;
      const cloned = req.clone({
        setHeaders: {
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
          'Content-Type': 'application/json',  
          'Accept':       'application/json'
        }
      });
      return next(cloned);
    }),
    catchError((error: HttpErrorResponse) => {
      // Só desloga em 401 — não em 404, 422, 500 etc.
      if (error.status === 401 && keycloak.isLoggedIn) {
        keycloak.logout();
      }
      return throwError(() => error);
    })
  );
};
