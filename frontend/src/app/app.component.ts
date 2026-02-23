import { Component, inject } from '@angular/core';
import { RouterOutlet }      from '@angular/router';
import { MatToolbarModule }  from '@angular/material/toolbar';
import { MatButtonModule }   from '@angular/material/button';
import { MatIconModule }     from '@angular/material/icon';
import { MatTooltipModule }  from '@angular/material/tooltip';
import { KeycloakService }   from './core/auth/keycloak.service';

@Component({
  selector:   'app-root',
  standalone: true,
  imports:    [
    RouterOutlet,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './app.component.html'
})
export class AppComponent {
  readonly keycloak = inject(KeycloakService);
}
