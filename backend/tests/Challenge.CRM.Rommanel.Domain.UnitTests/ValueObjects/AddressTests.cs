using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.ValueObjects;

public sealed class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var address = Address.Create("44073-200", "Av. Artemia Pires", "1000", "Registro", "Feira de Santana", "BA");
        address.PostalCode.Number.Should().Be("44073200");
        address.PostalCode.Formatted.Should().Be("44073-200");
        address.FederativeUnit.Abbreviation.Should().Be("BA");
        address.FederativeUnit.State.Should().Be("Bahia");
    }

    [Theory]
    [InlineData("0131010")]    // 7 dígitos
    [InlineData("013101000")]  // 9 dígitos
    public void Create_WithInvalidZip_ShouldThrowDomainException(string postalCode)
    {
        var act = () => Address.Create(postalCode, "Rua A", "1", "Bairro", "Cidade", "BA");
        act.Should().Throw<DomainException>().WithMessage($"'{postalCode}' não é um CEP válido. Informe 8 dígitos numéricos.");
    }

    [Fact]
    public void Equality_SameAddress_ShouldBeEqual()
    {
        var a = Address.Create("44073-200", "Av. Artemia Pires", "1000", "Registro", "Feira de Santana", "BA");
        var b = Address.Create("44073-200", "Av. Artemia Pires", "1000", "Registro", "Feira de Santana", "BA");
        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentAddress_ShouldNotBeEqual()
    {
        var a = Address.Create("41810010", "Rua A", "1", "Bairro", "Salvador", "BA");
        var b = Address.Create("41810010", "Rua B", "2", "Bairro", "Salvador", "BA");

        a.Should().NotBe(b);
    }

    [Theory]
    [InlineData("", "Rua", "1", "Bairro", "Cidade", "BA")]        // CEP vazio
    [InlineData("41810010", "", "1", "Bairro", "Cidade", "BA")]    // rua vazia
    [InlineData("41810010", "Rua", "1", "Bairro", "Cidade", "XX")] // UF inválida
    public void Create_WithInvalidData_ShouldThrowDomainException(
        string cep, string street, string num,
        string neighborhood, string city, string uf)
    {
        var act = () => Address.Create(cep, street, num, neighborhood, city, uf);
        act.Should().Throw<DomainException>();
    }
}