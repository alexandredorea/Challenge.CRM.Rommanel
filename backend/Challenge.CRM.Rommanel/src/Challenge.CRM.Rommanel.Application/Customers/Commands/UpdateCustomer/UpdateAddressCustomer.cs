using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateAddressCustomer(
    [property: JsonIgnore] Guid CustomerId,
    string PostalCode,
    string Street,
    string AddressNumber,
    string Neighborhood,
    string City,
    string FederativeUnit) : IRequest<Result<CustomerDto>>;