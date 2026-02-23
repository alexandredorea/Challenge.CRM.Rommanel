using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdHandler(IAppDbContext context)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FindAsync([query.CustomerId], cancellationToken)
            ?? throw new NotFoundException("Customer.NotFound", $"Cliente '{query.CustomerId}' não encontrado.");

        return Result<CustomerDto>.Ok(customer.ToDto());
    }
}