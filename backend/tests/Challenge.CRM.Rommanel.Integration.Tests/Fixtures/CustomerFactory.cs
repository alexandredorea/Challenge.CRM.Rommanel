using Bogus;
using Challenge.CRM.Rommanel.Application.Customers.Commands.CreateCustomer;

namespace Challenge.CRM.Rommanel.Integration.Tests.Fixtures;

/// <summary>
/// Gera dados de teste realistas usando Bogus.
/// </summary>
public static class CustomerFactory
{
    private static readonly Faker Faker = new("pt_BR");

    public static CreateCustomerCommand ValidPfCommand() =>
        new(
            Name: Faker.Name.FullName(),
            DocumentNumber: "529.982.247-25",
            BirthOrFoundationDate: new DateOnly(1990, 5, 15),
            Email: Faker.Internet.Email(),
            Telephone: "71999998888",
            PostalCode: "41810010",
            Street: Faker.Address.StreetName(),
            AddressNumber: Faker.Address.BuildingNumber(),
            Neighborhood: "Pituba",
            City: "Salvador",
            FederativeUnit: "BA",
            StateRegistration: null);

    public static CreateCustomerCommand ValidPjCommand() =>
        new(
            Name: Faker.Company.CompanyName(),
            DocumentNumber: "11.222.333/0001-81",
            BirthOrFoundationDate: new DateOnly(2010, 1, 20),
            Email: Faker.Internet.Email(),
            Telephone: "1133334444",
            PostalCode: "01310100",
            Street: Faker.Address.StreetName(),
            AddressNumber: Faker.Address.BuildingNumber(),
            Neighborhood: "Bela Vista",
            City: "São Paulo",
            FederativeUnit: "SP",
            StateRegistration: "110.042.490.114");
}