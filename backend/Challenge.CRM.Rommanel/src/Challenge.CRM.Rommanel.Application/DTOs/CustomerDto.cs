namespace Challenge.CRM.Rommanel.Application.DTOs;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string DocumentNumber,
    string DocumentFormatted,
    string DocumentType,
    DateOnly BirthOrFoundationDate,
    string Email,
    string Telephone,
    string TelephoneFormatted,
    AddressDto Address,
    string? StateRegistration,
    bool Active,
    DateTime CreatedAt
);