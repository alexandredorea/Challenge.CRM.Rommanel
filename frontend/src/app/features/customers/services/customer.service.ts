import { Injectable, inject }     from '@angular/core';
import { HttpClient, HttpParams }  from '@angular/common/http';
import { Observable }              from 'rxjs';
import { Result }                  from '../../../core/models/result.model';
import { PagedResult }             from '../../../core/models/paged-result.model';
import {
  CustomerDto,
  CustomerEventDto,
  CreateCustomerRequest
} from '../../../core/models/customer.model';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/customers';   // proxy/nginx resolve o host

  list(search?: string, page = 1, pageSize = 20)
    : Observable<Result<PagedResult<CustomerDto>>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);
    if (search?.trim())
      params = params.set('search', search.trim());
    return this.http.get<Result<PagedResult<CustomerDto>>>(this.base, { params });
  }

  getById(id: string): Observable<Result<CustomerDto>> {
    return this.http.get<Result<CustomerDto>>(`${this.base}/${id}`);
  }

  getEventHistory(id: string): Observable<Result<CustomerEventDto[]>> {
    return this.http.get<Result<CustomerEventDto[]>>(`${this.base}/${id}/events`);
  }

  create(request: CreateCustomerRequest): Observable<Result<CustomerDto>> {
    return this.http.post<Result<CustomerDto>>(this.base, request);
  }

  updateEmail(id: string, email: string): Observable<Result<CustomerDto>> {
    return this.http.patch<Result<CustomerDto>>(
      `${this.base}/${id}/email`, { email });
  }

  updateTelephone(id: string, telephone: string): Observable<Result<CustomerDto>> {
    return this.http.patch<Result<CustomerDto>>(
      `${this.base}/${id}/telephone`, { telephone });
  }

  updateAddress(id: string, address: {
    postalCode:     string;
    street:         string;
    number:         string;   
    neighborhood:   string;
    city:           string;
    federativeUnit: string;
  }): Observable<Result<CustomerDto>> {
    return this.http.patch<Result<CustomerDto>>(
      `${this.base}/${id}/address`, address);
  }

  disable(id: string): Observable<Result<CustomerDto>> {  
    return this.http.delete<Result<CustomerDto>>(`${this.base}/${id}`);
  }
}
