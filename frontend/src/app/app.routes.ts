import { Routes }    from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path:        'customers',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/customers/customers.routes')
        .then(m => m.CUSTOMERS_ROUTES)
  },
  { path: '', redirectTo: 'customers', pathMatch: 'full' },
  { path: '**', redirectTo: 'customers' }
];
