using Challenge.CRM.Rommanel.Application.DTOs;
using Challenge.CRM.Rommanel.Domain.Entities;
using Challenge.CRM.Rommanel.Domain.ValueObjects;

namespace Challenge.CRM.Rommanel.Application.Customers;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new(
            Id: customer.Id,
            Name: customer.Name,
            DocumentNumber: customer.Document.Number,
            DocumentFormatted: customer.Document.Formatted,
            DocumentType: customer.Document.Type.ToString(),
            BirthOrFoundationDate: customer.OriginDate,
            Email: customer.Email.Address,
            Telephone: customer.Telephone.Number,
            TelephoneFormatted: customer.Telephone.FormattedNumber,
            Address: customer.Address.ToDto(),
            StateRegistration: customer.StateRegistration,
            Active: customer.Active,
            CreatedAt: customer.CreatedAt);
    }

    private static AddressDto ToDto(this Address address) =>
        new(
            PostalCode: address.PostalCode.Formatted,
            Street: address.Street,
            Number: address.Number,
            Neighborhood: address.Neighborhood,
            City: address.City,
            FederativeUnit: address.FederativeUnit.Abbreviation);
}