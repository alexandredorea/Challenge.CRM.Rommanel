using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.ValueObjects;

public sealed class FederativeUnitTests
{
    [Theory]
    [InlineData("BA", "Bahia")]
    [InlineData("SP", "São Paulo")]
    [InlineData("ba", "Bahia")] // case insensitive
    public void Create_WithValidAbbreviation_ShouldResolveStateName(
        string input, string expectedName)
    {
        var uf = FederativeUnit.Create(input);

        uf.Abbreviation.Should().Be(input.ToUpperInvariant());
        uf.State.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("Bahia")]
    [InlineData("São Paulo")]
    public void Create_WithFullStateName_ShouldResolveAbbreviation(string stateName)
    {
        var uf = FederativeUnit.Create(stateName);
        uf.Abbreviation.Should().HaveLength(2);
    }

    [Theory]
    [InlineData("XX")]
    [InlineData("ZZ")]
    [InlineData("")]
    public void Create_WithInvalidValue_ShouldThrowDomainException(string value)
    {
        var act = () => FederativeUnit.Create(value);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_SameAbbreviation_ShouldBeEqual()
    {
        var a = FederativeUnit.Create("BA");
        var b = FederativeUnit.Create("Bahia");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }
}
