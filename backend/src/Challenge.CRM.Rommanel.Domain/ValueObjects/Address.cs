using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Primitives;

namespace Challenge.CRM.Rommanel.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um endereço completo.
/// </summary>
public sealed class Address : ValueObject
{
    public PostalCode PostalCode { get; } = default!;
    public string Street { get; } = string.Empty;
    public string Number { get; } = string.Empty;
    public string Neighborhood { get; } = string.Empty;
    public string City { get; } = string.Empty;
    public FederativeUnit FederativeUnit { get; } = default!;

    private Address()
    {
    }

    private Address(
        PostalCode postalCode,
        string street,
        string number,
        string neighborhood,
        string city,
        FederativeUnit federativeUnit)
    {
        PostalCode = postalCode;
        FederativeUnit = federativeUnit;

        Street = Normalize(street, nameof(Street));
        Number = Normalize(number, nameof(Number));
        Neighborhood = Normalize(neighborhood, nameof(Neighborhood));
        City = Normalize(city, nameof(City));
    }

    public static Address Create(
        string postalCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string federativeUnit)
    {
        return new Address(
            PostalCode.Create(postalCode),
            street,
            number,
            neighborhood,
            city,
            FederativeUnit.Create(federativeUnit));
    }

    private static string Normalize(string value, string errorCode)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(errorCode, "Campo obrigatório.");

        return value.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PostalCode;
        yield return Street;
        yield return Number;
        yield return Neighborhood;
        yield return City;
        yield return FederativeUnit;
    }

    public override string ToString()
        => $"{Street}, {Number} — {Neighborhood}, {City}/{FederativeUnit} — {PostalCode}";
}
