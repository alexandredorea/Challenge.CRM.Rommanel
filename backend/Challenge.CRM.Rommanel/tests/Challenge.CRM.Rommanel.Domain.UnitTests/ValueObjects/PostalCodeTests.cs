using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.ValueObjects;

public sealed class PostalCodeTests
{
    [Theory]
    [InlineData("41810-010")]
    [InlineData("41810010")]
    [InlineData("01310100")]
    public void Create_WithValidPostalCode_ShouldSucceed(string value)
    {
        var postalCode = PostalCode.Create(value);
        postalCode.Number.Length.Should().Be(8);
        postalCode.Formatted.Should().Contain("-");
    }

    [Theory]
    [InlineData("00000-000")] // fora da faixa
    [InlineData("00000000")] // zeros
    [InlineData("1234")]     // curto demais
    [InlineData("")]         // vazio
    public void Create_WithInvalidPostalCode_ShouldThrowDomainException(string value)
    {
        var act = () => PostalCode.Create(value);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Formatted_ShouldApplyMask()
    {
        var pc = PostalCode.Create("41810010");
        pc.Formatted.Should().Be("41810-010");
    }
}