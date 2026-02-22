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
    public void Equality_SameValues_ShouldBeEqual()
    {
        var a = Address.Create("44073-200", "Av. Artemia Pires", "1000", "Registro", "Feira de Santana", "BA");
        var b = Address.Create("44073-200", "Av. Artemia Pires", "1000", "Registro", "Feira de Santana", "BA");
        a.Should().Be(b);
    }
}