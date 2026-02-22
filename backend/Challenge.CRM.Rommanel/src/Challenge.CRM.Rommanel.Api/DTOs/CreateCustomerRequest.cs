namespace Challenge.CRM.Rommanel.Api.DTOs;

public sealed record CreateCustomerRequest(
    string Name,
    string Document,
    DateOnly BirthOrFoundationDate,
    string Email,
    string Telephone,
    string PostalCode,
    string Street,
    string AddressNumber,
    string Neighborhood,
    string City,
    string FederativeUnit,
    string? StateRegistration = null);