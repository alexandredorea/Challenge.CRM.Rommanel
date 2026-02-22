import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router }  from '@angular/router';
import { CustomerService }         from '../../services/customer.service';
import { CustomerEventDto }        from '../../../../core/models/customer.model';
import { SharedMaterialModule }    from '../../../../shared/material.module';
import { DatePipe, JsonPipe }      from '@angular/common';

@Component({
  selector:    'app-customer-events',
  standalone:  true,
  imports:     [SharedMaterialModule, DatePipe, JsonPipe],
  templateUrl: './customer-events.component.html'
})
export class CustomerEventsComponent implements OnInit {
  private readonly service = inject(CustomerService);
  private readonly route   = inject(ActivatedRoute);
  private readonly router  = inject(Router);

  readonly loading = signal(false);
  readonly events  = signal<CustomerEventDto[]>([]);
  readonly name    = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);

    this.service.getById(id).subscribe(r => {
      if (r.success && r.data) this.name.set(r.data.name);
    });

    this.service.getEventHistory(id).subscribe({
      next:     r => { if (r.success && r.data) this.events.set(r.data); },
      complete: () => this.loading.set(false)
    });
  }

  eventIcon(type: string): string {
    const map: Record<string, string> = {
      CustomerCreated:                  'person_add',
      CustomerEmailChanged:             'email',
      CustomerTelephoneChanged:         'phone',
      CustomerAddressChanged:           'home',
      CustomerStateRegistrationChanged: 'receipt_long',
      CustomerDisabled:                 'block'
    };
    return map[type] ?? 'event';
  }

  eventLabel(type: string): string {
    const map: Record<string, string> = {
      CustomerCreated:                  'Cliente cadastrado',
      CustomerEmailChanged:             'E-mail alterado',
      CustomerTelephoneChanged:         'Telefone alterado',
      CustomerAddressChanged:           'Endereço atualizado',
      CustomerStateRegistrationChanged: 'Inscrição Estadual atualizada',
      CustomerDisabled:                 'Cliente desativado'
    };
    return map[type] ?? type;
  }

  back(): void { this.router.navigate(['/customers']); }
}
