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
    string? StateRegistration,
    bool IsStateRegistrationExempt
);

public sealed record UpdateCustomerEmailRequest(string Email);

public sealed record UpdateCustomerTelephoneRequest(string Telephone);

public sealed record UpdateCustomerAddressRequest(
    string PostalCode,
    string Street,
    string AddressNumber,
    string Neighborhood,
    string City,
    string FederativeUnit
);