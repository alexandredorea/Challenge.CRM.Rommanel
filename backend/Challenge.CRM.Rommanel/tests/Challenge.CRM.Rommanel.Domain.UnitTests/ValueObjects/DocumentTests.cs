using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.ValueObjects;

public sealed class DocumentTests
{
    [Theory]
    [InlineData("529.982.247-25", DocumentType.Individual)]
    [InlineData("52998224725", DocumentType.Individual)]
    [InlineData("11.222.333/0001-81", DocumentType.LegalEntity)]
    [InlineData("11222333000181", DocumentType.LegalEntity)]
    public void Create_WithValidDocument_ShouldReturnCorrectType(string raw, DocumentType expected)
    {
        var doc = Document.Create(raw);
        doc.Type.Should().Be(expected);
    }

    [Theory]
    [InlineData("000.000.000-00")]
    [InlineData("111.111.111-11")]
    [InlineData("123.456.789-00")]
    [InlineData("99999999999999")]
    public void Create_WithInvalidDocument_ShouldThrowDomainException(string raw)
    {
        var act = () => Document.Create(raw);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_ShouldStoreOnlyDigits()
    {
        var doc = Document.Create("529.982.247-25");
        doc.Number.Should().Be("52998224725");
    }

    [Fact]
    public void Equality_SameDocument_ShouldBeEqual()
    {
        var a = Document.Create("52998224725");
        var b = Document.Create("529.982.247-25");
        a.Should().Be(b);
    }
}