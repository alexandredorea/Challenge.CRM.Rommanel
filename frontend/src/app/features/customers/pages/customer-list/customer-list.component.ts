import {
  Component, OnInit, inject, signal, computed
} from '@angular/core';
import { Router }           from '@angular/router';
import { FormsModule }      from '@angular/forms';
import { MatDialog }        from '@angular/material/dialog';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CustomerService }  from '../../services/customer.service';
import { CustomerDto }      from '../../../../core/models/customer.model';
import { PagedResult }      from '../../../../core/models/paged-result.model';
import { SharedMaterialModule } from '../../../../shared/material.module';
import { ConfirmDialogComponent }
  from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector:    'app-customer-list',
  standalone:  true,
  imports:     [SharedMaterialModule, FormsModule],
  templateUrl: './customer-list.component.html'
})
export class CustomerListComponent implements OnInit {
  private readonly service = inject(CustomerService);
  private readonly router  = inject(Router);
  private readonly dialog  = inject(MatDialog);

  readonly loading   = signal(false);
  readonly customers = signal<PagedResult<CustomerDto> | null>(null);
  readonly search    = signal('');

  readonly displayedColumns = [
    'name', 'documentFormatted', 'documentType',
    'email', 'telephoneFormatted', 'status', 'actions'
  ];

  private readonly searchSubject = new Subject<string>();

  page     = 1;
  pageSize = 20;

  ngOnInit(): void {
    this.load();

    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.page = 1;
      this.load();
    });
  }

  load(): void {
    this.loading.set(true);
    this.service.list(this.search(), this.page, this.pageSize)
      .subscribe({
        next:  r => { if (r.success) this.customers.set(r.data); },
        error: () => {},
        complete: () => this.loading.set(false)
      });
  }

  onSearchChange(value: string): void {
    this.search.set(value);
    this.searchSubject.next(value);
  }

  onPageChange(event: { pageIndex: number; pageSize: number }): void {
    this.page     = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  create(): void { this.router.navigate(['/customers/new']); }

  edit(id: string): void { this.router.navigate(['/customers', id, 'edit']); }

  viewEvents(id: string): void { this.router.navigate(['/customers', id, 'events']); }

  disable(customer: CustomerDto): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title:   'Desativar cliente',
        message: `Deseja desativar o cliente "${customer.name}"? Esta ação não pode ser desfeita.`,
        confirm: 'Desativar',
        warn:    true
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.service.disable(customer.id).subscribe(() => this.load());
    });
  }
}
