using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace Challenge.CRM.Rommanel.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(
    [property: JsonIgnore] Guid CustomerId) : IRequest<Result<CustomerDto>>;