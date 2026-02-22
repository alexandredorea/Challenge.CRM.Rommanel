using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.DeleteCustomer;

public sealed record DisableCustomerCommand(Guid CustomerId) : IRequest<Result<CustomerDto>>;