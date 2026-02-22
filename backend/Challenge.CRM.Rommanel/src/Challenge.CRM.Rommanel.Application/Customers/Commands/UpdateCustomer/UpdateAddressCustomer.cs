using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateAddressCustomer(
    Guid CustomerId,
    string PostalCode,
    string Street,
    string AddressNumber,
    string Neighborhood,
    string City,
    string FederativeUnit) : IRequest<Result<CustomerDto>>;