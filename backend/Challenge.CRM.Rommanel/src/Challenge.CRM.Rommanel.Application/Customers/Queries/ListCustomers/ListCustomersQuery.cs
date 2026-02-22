using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.ListCustomers;

public sealed record ListCustomersQuery(string? Search, int Page = 1, int PageSize = 20) : IRequest<Result<PagedResult<CustomerDto>>>;