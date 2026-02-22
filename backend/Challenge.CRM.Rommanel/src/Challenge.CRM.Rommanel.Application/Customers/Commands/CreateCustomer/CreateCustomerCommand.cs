using Challenge.CRM.Rommanel.Application.Common.Models;
using Challenge.CRM.Rommanel.Application.DTOs;
using MediatR;

namespace Challenge.CRM.Rommanel.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string DocumentNumber,
    DateOnly BirthOrFoundationDate,
    string Email,
    string Telephone,
    string PostalCode,
    string Street,
    string AddressNumber,
    string Neighborhood,
    string City,
    string FederativeUnit,
    string? StateRegistration) : IRequest<Result<CustomerDto>>;