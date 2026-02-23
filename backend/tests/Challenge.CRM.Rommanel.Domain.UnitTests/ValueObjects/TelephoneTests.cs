using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.ValueObjects;

public sealed class TelephoneTests
{
    [Theory]
    [InlineData("71999998888")]
    [InlineData("(71) 99999-8888")]
    [InlineData("1133334444")]
    public void Create_WithValidNumber_ShouldSucceed(string value)
    {
        var tel = Telephone.Create(value);

        tel.FormattedNumber.Should().Contain("-");
    }

    [Theory]
    [InlineData("00999998888")] // DDD inválido
    [InlineData("7199998888")]  // celular com 8 inicial inválido
    [InlineData("123")]         // curto demais
    [InlineData("")]            // vazio
    public void Create_WithInvalidNumber_ShouldThrowDomainException(string value)
    {
        var act = () => Telephone.Create(value);
        act.Should().Throw<DomainException>();
    }
}