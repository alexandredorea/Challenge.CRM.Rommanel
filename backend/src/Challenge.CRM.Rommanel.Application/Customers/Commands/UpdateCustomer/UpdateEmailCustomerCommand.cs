using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateEmailCustomerCommand(
    [property: JsonIgnore] Guid CustomerId,
    string Email) : IRequest<Result<CustomerDto>>;