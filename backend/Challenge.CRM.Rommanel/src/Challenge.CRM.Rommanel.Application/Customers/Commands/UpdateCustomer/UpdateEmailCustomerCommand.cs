using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateEmailCustomerCommand(Guid CustomerId, string Email) : IRequest<Result<CustomerDto>>;