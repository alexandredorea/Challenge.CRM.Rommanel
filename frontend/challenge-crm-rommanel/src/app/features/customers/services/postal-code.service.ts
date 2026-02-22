import { Injectable, inject } from '@angular/core';
import { HttpClient }         from '@angular/common/http';
import { Observable, of }     from 'rxjs';
import { catchError, map }    from 'rxjs/operators';
import { environment }        from '../../../../environments/environment';
import { Result }             from '../../../core/models/result.model';

export interface PostalCodeResult {
  logradouro: string;
  bairro:     string;
  localidade: string;
  uf:         string;
}

@Injectable({ providedIn: 'root' })
export class PostalCodeService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/address`;

  lookup(postalCode: string): Observable<PostalCodeResult | null> {
    const digits = postalCode.replace(/\D/g, '');

    if (digits.length !== 8) return of(null);

    return this.http
      .get<Result<PostalCodeResult>>(`${this.base}/${digits}`)
      .pipe(
        map(r  => r.success ? r.data : null),
        catchError(() => of(null))
      );
  }
}
