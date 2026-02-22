using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.ValueObjects;

public sealed class EmailTests
{
    [Theory]
    [InlineData("joao@email.com")]
    [InlineData("JOAO@EMAIL.COM")]   // normaliza para lowercase
    [InlineData("user+tag@domain.org")]
    public void Create_WithValidEmail_ShouldSucceed(string value)
    {
        var email = Email.Create(value);
        email.Address.Should().Be(value.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("naoeumemail")]
    [InlineData("@semlocal.com")]
    [InlineData("sem@")]
    public void Create_WithInvalidEmail_ShouldThrowDomainException(string value)
    {
        var act = () => Email.Create(value);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_SameAddress_ShouldBeEqual()
    {
        var a = Email.Create("a@b.com");
        var b = Email.Create("A@B.COM");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentAddresses_ShouldNotBeEqual()
    {
        var a = Email.Create("a@b.com");
        var b = Email.Create("c@d.com");

        (a != b).Should().BeTrue();
    }
}