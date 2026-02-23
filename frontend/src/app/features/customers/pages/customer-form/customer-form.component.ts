import {
  Component, OnInit, inject, signal
} from '@angular/core';
import {
  FormBuilder, FormGroup, Validators, ReactiveFormsModule
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, Observable, of } from 'rxjs';
import {
  debounceTime, distinctUntilChanged,
  filter, switchMap
} from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CustomerService } from '../../services/customer.service';
import { PostalCodeService } from '../../services/postal-code.service';
import { CustomerDto } from '../../../../core/models/customer.model';
import { Result, ResultError } from '../../../../core/models/result.model';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './customer-form.component.html'
})
export class CustomerFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(CustomerService);
  private readonly postalSvc = inject(PostalCodeService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snackBar = inject(MatSnackBar);

  readonly loading = signal(false);
  readonly isEditMode = signal(false);
  readonly isLegalEntity = signal(false);
  readonly loadingCep = signal(false);
  readonly errors = signal<ResultError[]>([]);

  readonly ufs = [
    'AC', 'AL', 'AM', 'AP', 'BA', 'CE', 'DF', 'ES', 'GO',
    'MA', 'MG', 'MS', 'MT', 'PA', 'PB', 'PE', 'PI', 'PR',
    'RJ', 'RN', 'RO', 'RR', 'RS', 'SC', 'SE', 'SP', 'TO'
  ];

  customerId: string | null = null;
  form!: FormGroup;

  // Guarda dados originais para comparar no modo edição
  private originalData: CustomerDto | null = null;

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
        const isPj = digits.length > 11;
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
          street: result.street,
          neighborhood: result.neighborhood,
          city: result.city,
          federativeUnit: result.federativeUnit
        });
      });
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]],
      document: ['', Validators.required],
      birthOrFoundationDate: ['', Validators.required],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
      telephone: ['', Validators.required],
      postalCode: ['', Validators.required],
      street: ['', Validators.required],
      addressNumber: ['', Validators.required],
      neighborhood: ['', Validators.required],
      city: ['', Validators.required],
      federativeUnit: ['', Validators.required],
      stateRegistration: [null]
    });
  }

  private updateStateRegistrationValidators(isPj: boolean): void {
    const ctrl = this.form.get('stateRegistration')!;
    if (isPj)
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
        this.originalData = r.data;
        const c = r.data;
        this.form.patchValue({
          name: c.name,
          document: c.documentFormatted,
          birthOrFoundationDate: c.birthOrFoundationDate,
          email: c.email,
          telephone: c.telephoneFormatted,
          postalCode: c.address.postalCode,
          street: c.address.street,
          addressNumber: c.address.number,
          neighborhood: c.address.neighborhood,
          city: c.address.city,
          federativeUnit: c.address.federativeUnit,
          stateRegistration: c.stateRegistration
        });
        this.form.get('document')!.disable();
      },
      complete: () => this.loading.set(false)
    });
  }

  onExemptChange(checked: boolean): void {
    this.updateStateRegistrationValidators(this.isLegalEntity() && !checked);
  }

  private formatDateOnly(value: string | Date | null): string {
    if (!value) return '';
    const d = new Date(value);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.errors.set([]);

    if (this.isEditMode()) {
      this.submitUpdate();
    } else {
      this.submitCreate();
    }
  }

  private submitCreate(): void {
    const v = this.form.getRawValue();
    this.service.create({
      name: v.name,
      document: v.document,
      birthOrFoundationDate: this.formatDateOnly(v.birthOrFoundationDate),
      email: v.email,
      telephone: v.telephone,
      postalCode: v.postalCode,
      street: v.street,
      addressNumber: v.addressNumber,
      neighborhood: v.neighborhood,
      city: v.city,
      federativeUnit: v.federativeUnit,
      stateRegistration: v.stateRegistration || null
    }).subscribe({
      next: r => {
        if (r.success) {
          this.snackBar.open('Cliente cadastrado!', 'Fechar',
            { duration: 3000, panelClass: 'snack-success' });
          this.router.navigate(['/customers']);
        } else {
          this.errors.set(r.errors ?? []);
          this.loading.set(false);
        }
      },
      error: () => {
        this.snackBar.open('Erro de conexão', 'Fechar',
          { duration: 4000, panelClass: 'snack-error' });
        this.loading.set(false);
      }
    });
  }

  private submitUpdate(): void {
    const v = this.form.getRawValue();
    const orig = this.originalData;
    if (!orig || !this.customerId) return;

    const calls: Observable<Result<CustomerDto>>[] = [];

    if (v.email !== orig.email)
      calls.push(this.service.updateEmail(this.customerId, v.email));

    if (v.telephone?.replace(/\D/g, '') !== orig.telephone?.replace(/\D/g, ''))
      calls.push(this.service.updateTelephone(this.customerId, v.telephone));

    const addrChanged =
      v.postalCode !== orig.address.postalCode ||
      v.street !== orig.address.street ||
      v.addressNumber !== orig.address.number ||
      v.neighborhood !== orig.address.neighborhood ||
      v.city !== orig.address.city ||
      v.federativeUnit !== orig.address.federativeUnit;

    if (addrChanged)
      calls.push(this.service.updateAddress(this.customerId, {
        postalCode: v.postalCode,
        street: v.street,
        number: v.addressNumber,   
        neighborhood: v.neighborhood,
        city: v.city,
        federativeUnit: v.federativeUnit
      }));

    if (calls.length === 0) {
      this.snackBar.open('Nenhuma alteração detectada', 'Fechar', { duration: 3000 });
      this.loading.set(false);
      return;
    }

    forkJoin(calls).subscribe({
      next: results => {
        // Verifica se alguma chamada retornou erro de negócio
        const failed = results.find(r => !r.success);
        if (failed) {
          this.errors.set(failed.errors ?? []);
          this.loading.set(false);
          return;
        }
        this.snackBar.open('Cliente atualizado!', 'Fechar',
          { duration: 3000, panelClass: 'snack-success' });
        this.router.navigate(['/customers']);
      },
      error: () => {
        this.snackBar.open('Erro ao atualizar', 'Fechar',
          { duration: 4000, panelClass: 'snack-error' });
        this.loading.set(false);
      }
    });
  }

  cancel(): void { this.router.navigate(['/customers']); }

  getError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl?.touched || !ctrl.errors) return '';
    if (ctrl.errors['required']) return 'Campo obrigatório.';
    if (ctrl.errors['email']) return 'E-mail inválido.';
    if (ctrl.errors['maxlength']) return 'Tamanho máximo excedido.';
    return '';
  }
}
