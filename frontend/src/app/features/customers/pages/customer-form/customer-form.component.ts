import {
  Component, OnInit, inject, signal
} from '@angular/core';
import {
  FormBuilder, FormGroup, Validators,
  ReactiveFormsModule, AbstractControl
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { CustomerService }   from '../../services/customer.service';
import { PostalCodeService } from '../../services/postal-code.service';
import { SharedMaterialModule } from '../../../../shared/material/material.module';

@Component({
  selector:    'app-customer-form',
  standalone:  true,
  imports:     [SharedMaterialModule, ReactiveFormsModule],
  templateUrl: './customer-form.component.html'
})
export class CustomerFormComponent implements OnInit {
  private readonly fb         = inject(FormBuilder);
  private readonly service    = inject(CustomerService);
  private readonly postalSvc  = inject(PostalCodeService);
  private readonly router     = inject(Router);
  private readonly route      = inject(ActivatedRoute);

  readonly loading     = signal(false);
  readonly isEditMode  = signal(false);
  readonly isLegalEntity = signal(false);
  readonly loadingCep  = signal(false);
  readonly errors      = signal<string[]>([]);
  readonly ufs = [
    'AC','AL','AM','AP','BA','CE','DF','ES','GO',
    'MA','MG','MS','MT','PA','PB','PE','PI','PR',
    'RJ','RN','RO','RR','RS','SC','SE','SP','TO'
  ];
  
  customerId: string | null = null;

  form!: FormGroup;

  ngOnInit(): void {
    this.buildForm();

    this.customerId = this.route.snapshot.paramMap.get('id');
    if (this.customerId) {
      this.isEditMode.set(true);
      this.loadCustomer(this.customerId);
    }

    // Detecta tipo de pessoa pelo documento
    this.form.get('document')!.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(value => {
        const digits = (value ?? '').replace(/\D/g, '');
        const isPj   = digits.length > 11;
        this.isLegalEntity.set(isPj);
        this.updateStateRegistrationValidators(isPj);
      });

    // Busca CEP automaticamente
    this.form.get('postalCode')!.valueChanges
      .pipe(
        debounceTime(600),
        distinctUntilChanged(),
        filter(v => v?.replace(/\D/g, '').length === 8),
        switchMap(v => {
          this.loadingCep.set(true);
          return this.postalSvc.lookup(v);
        })
      )
      .subscribe(result => {
        this.loadingCep.set(false);
        if (!result) return;
        this.form.patchValue({
          street:        result.logradouro,
          neighborhood:  result.bairro,
          city:          result.localidade,
          federativeUnit: result.uf
        });
      });
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name:                      ['', [Validators.required, Validators.maxLength(255)]],
      document:                  ['', Validators.required],
      birthOrFoundationDate:     ['', Validators.required],
      email:                     ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
      telephone:                 ['', Validators.required],
      postalCode:                ['', Validators.required],
      street:                    ['', Validators.required],
      addressNumber:             ['', Validators.required],
      neighborhood:              ['', Validators.required],
      city:                      ['', Validators.required],
      federativeUnit:            ['', Validators.required],
      stateRegistration:         [null],
      isStateRegistrationExempt: [false]
    });
  }

  private updateStateRegistrationValidators(isPj: boolean): void {
    const ctrl = this.form.get('stateRegistration')!;
    const exempt = this.form.get('isStateRegistrationExempt')!.value;

    if (isPj && !exempt)
      ctrl.setValidators(Validators.required);
    else
      ctrl.clearValidators();

    ctrl.updateValueAndValidity();
  }

  private loadCustomer(id: string): void {
    this.loading.set(true);
    this.service.getById(id).subscribe({
      next: r => {
        if (!r.success || !r.data) return;
        const c = r.data;
        this.form.patchValue({
          name:                  c.name,
          document:              c.documentFormatted,
          birthOrFoundationDate: c.birthOrFoundationDate,
          email:                 c.email,
          telephone:             c.telephoneFormatted,
          postalCode:            c.address.postalCode,
          street:                c.address.street,
          addressNumber:         c.address.number,
          neighborhood:          c.address.neighborhood,
          city:                  c.address.city,
          federativeUnit:        c.address.federativeUnit,
          stateRegistration:     c.stateRegistration
        });
        this.form.get('document')!.disable();
      },
      complete: () => this.loading.set(false)
    });
  }


  onExemptChange(checked: boolean): void {
    this.updateStateRegistrationValidators(this.isLegalEntity() && !checked);
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.loading.set(true);
    this.errors.set([]);

    const value = this.form.getRawValue();

    const request = {
      name:                      value.name,
      document:                  value.document,
      birthOrFoundationDate:     value.birthOrFoundationDate,
      email:                     value.email,
      telephone:                 value.telephone,
      postalCode:                value.postalCode,
      street:                    value.street,
      addressNumber:             value.addressNumber,
      neighborhood:              value.neighborhood,
      city:                      value.city,
      federativeUnit:            value.federativeUnit,
      stateRegistration:         value.stateRegistration || null,
      isStateRegistrationExempt: value.isStateRegistrationExempt
    };

    const op$ = this.isEditMode()
      ? this.service.updateContact(this.customerId!, value.email, value.telephone)
      : this.service.create(request);

    op$.subscribe({
      next: r => {
        if (r.success) {
          this.router.navigate(['/customers']);
        } else {
          this.errors.set(r.errors.map(e => e.message));
        }
      },
      complete: () => this.loading.set(false)
    });
  }

  cancel(): void { this.router.navigate(['/customers']); }

  getError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl?.touched || !ctrl.errors) return '';
    if (ctrl.errors['required'])  return 'Campo obrigatório.';
    if (ctrl.errors['email'])     return 'E-mail inválido.';
    if (ctrl.errors['maxlength']) return 'Tamanho máximo excedido.';
    return '';
  }
}
