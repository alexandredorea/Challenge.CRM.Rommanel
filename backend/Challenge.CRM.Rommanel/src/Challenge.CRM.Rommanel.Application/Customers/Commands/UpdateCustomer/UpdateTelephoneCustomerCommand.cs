using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateTelephoneCustomerCommand(Guid CustomerId, string Telephone) : IRequest<Result<CustomerDto>>;