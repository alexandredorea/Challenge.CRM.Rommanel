import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject }  from '@angular/core';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { KeycloakService } from '../auth/keycloak.service';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const keycloak = inject(KeycloakService);

  // Não intercepta chamadas externas (ex: ViaCEP direto)
  if (!req.url.includes('/api/')) return next(req);

  return from(keycloak.refreshToken()).pipe(
    switchMap(() => {
      const token   = keycloak.token;
      const cloned  = token
        ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
        : req;
      return next(cloned);
    }),
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) keycloak.logout();
      return throwError(() => error);
    })
  );
};
