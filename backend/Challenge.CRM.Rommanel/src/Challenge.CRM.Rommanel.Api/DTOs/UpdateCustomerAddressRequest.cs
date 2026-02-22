namespace Challenge.CRM.Rommanel.Api.DTOs;

public sealed record UpdateCustomerAddressRequest(
    string PostalCode,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string FederativeUnit
);