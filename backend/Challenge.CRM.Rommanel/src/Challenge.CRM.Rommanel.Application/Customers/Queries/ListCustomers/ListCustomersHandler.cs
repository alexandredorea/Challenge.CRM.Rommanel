using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.ListCustomers;

public sealed class ListCustomersHandler(IAppDbContext context) : IRequestHandler<ListCustomersQuery, Result<PagedResult<CustomerDto>>>
{
    public async Task<Result<PagedResult<CustomerDto>>> Handle(
        ListCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = context.Customers.AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(c => c.ToDto())
            .AsQueryable();

        //TODO: melhorar para campo computado (usar conceito de full text search)
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim().ToLowerInvariant();
            customers = customers.Where(e =>
                e.Name.ToLower().Contains(s) ||
                e.DocumentNumber.Contains(s) ||
                e.Email.ToLower().Contains(s) ||
                e.Address.PostalCode.Contains(s));
        }

        var result = await customers.ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

        return Result<PagedResult<CustomerDto>>.Ok(result);
    }
}