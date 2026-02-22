namespace Challenge.CRM.Rommanel.Application.DTOs;

public sealed record AddressDto(
string PostalCode,
string Street,
string Number,
string Neighborhood,
string City,
string FederativeUnit)
{
}