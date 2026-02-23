import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Result }        from '../../../core/models/result.model';
import { PagedResult }   from '../../../core/models/paged-result.model';
import {
  CustomerDto,
  CustomerEventDto,
  CreateCustomerRequest
} from '../../../core/models/customer.model';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/v1/customers`;

  list(search?: string, page = 1, pageSize = 20):
    Observable<Result<PagedResult<CustomerDto>>> {

    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (search?.trim())
      params = params.set('search', search.trim());

    return this.http.get<Result<PagedResult<CustomerDto>>>(
      this.base, { params });
  }

  getById(id: string): Observable<Result<CustomerDto>> {
    return this.http.get<Result<CustomerDto>>(`${this.base}/${id}`);
  }

  getEventHistory(id: string): Observable<Result<CustomerEventDto[]>> {
    return this.http.get<Result<CustomerEventDto[]>>(
      `${this.base}/${id}/events`);
  }

  create(request: CreateCustomerRequest): Observable<Result<CustomerDto>> {
    return this.http.post<Result<CustomerDto>>(this.base, request);
  }

  updateContact(id: string, email: string, telephone: string):
    Observable<Result<CustomerDto>> {
    return this.http.patch<Result<CustomerDto>>(
      `${this.base}/${id}/contact`, { email, telephone });
  }

  updateAddress(id: string, address: {
    postalCode:    string;
    street:        string;
    addressNumber: string;
    neighborhood:  string;
    city:          string;
    federativeUnit: string;
  }): Observable<Result<CustomerDto>> {
    return this.http.patch<Result<CustomerDto>>(
      `${this.base}/${id}/address`, address);
  }

  disable(id: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.base}/${id}`);
  }
}
