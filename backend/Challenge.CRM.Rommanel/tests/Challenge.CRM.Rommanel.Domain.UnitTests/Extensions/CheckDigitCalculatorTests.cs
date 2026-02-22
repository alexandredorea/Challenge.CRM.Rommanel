using Challenge.CRM.Rommanel.Domain.Extensions;
using FluentAssertions;

namespace Challenge.CRM.Rommanel.Domain.UnitTests.Extensions;

public sealed class CheckDigitCalculatorTests
{
    [Theory]
    [InlineData("529982247", "25")] // CPF válido
    [InlineData("112223330001", "81")] // CNPJ válido
    public void Calculate_ShouldReturnCorrectDigits(
        string number, string expectedDigits)
    {
        // Para CPF: calcula primeiro dígito com multiplicadores 10..2
        // Para CNPJ: calcula primeiro dígito com multiplicadores explícitos
        var isCpf = number.Length == 9;

        string first, second;

        if (isCpf)
        {
            var calc = new CheckDigitCalculator(number)
                .WithMultipliers(10, 2)
                .ReplaceWith("0", 10, 11);
            first = calc.Calculate();
            second = calc.AddDigit(first).WithMultipliers(11, 2).Calculate();
        }
        else
        {
            var calc = new CheckDigitCalculator(number)
                .WithMultipliers(5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2)
                .ReplaceWith("0", 10, 11);
            first = calc.Calculate();
            second = calc.AddDigit(first)
                .WithMultipliers(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2)
                .Calculate();
        }

        $"{first}{second}".Should().Be(expectedDigits);
    }

    [Fact]
    public void Constructor_WhenNullOrEmpty_ShouldThrow()
    {
        var act = () => new CheckDigitCalculator(string.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithMultipliers_WhenEmpty_ShouldThrow()
    {
        var act = () => new CheckDigitCalculator("123")
            .WithMultipliers(Array.Empty<int>());
        act.Should().Throw<ArgumentException>();
    }
}

public sealed class DocumentExtensionsTests
{
    // ─── CPF ──────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("52998224725", true)]
    [InlineData("111.111.111-11", false)] // todos iguais
    [InlineData("000.000.000-00", false)] // todos zeros
    [InlineData("529.982.247-26", false)] // dígito errado
    [InlineData("123.456.789-00", false)] // inválido
    [InlineData("", false)] // vazio
    public void IsValidCpf_ShouldReturnExpected(string cpf, bool expected) =>
        cpf.IsValidCpf().Should().Be(expected);

    // ─── CNPJ ─────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("11.222.333/0001-81", true)]
    [InlineData("11222333000181", true)]
    [InlineData("11.111.111/1111-11", false)] // todos iguais
    [InlineData("11.222.333/0001-82", false)] // dígito errado
    [InlineData("", false)] // vazio
    public void IsValidCnpj_ShouldReturnExpected(string cnpj, bool expected) =>
        cnpj.IsValidCnpj().Should().Be(expected);

    // ─── Formatação ───────────────────────────────────────────────────────────

    [Fact]
    public void ToCpfFormatted_ShouldFormatCorrectly() =>
        "52998224725".ToCpfFormatted().Should().Be("529.982.247-25");

    [Fact]
    public void ToCnpjFormatted_ShouldFormatCorrectly() =>
        "11222333000181".ToCnpjFormatted().Should().Be("11.222.333/0001-81");

    [Fact]
    public void ToCpfFormatted_WhenInvalidLength_ShouldReturnEmpty() =>
        "123".ToCpfFormatted().Should().BeEmpty();
}

public sealed class TelephoneExtensionsTests
{
    [Theory]
    [InlineData("71999998888", true)]  // celular BA
    [InlineData("(71) 99999-8888", true)] // celular com máscara
    [InlineData("1133334444", true)]  // fixo SP
    [InlineData("71888887777", false)] // celular com 8 inicial inválido
    [InlineData("00999998888", false)] // DDD inválido
    [InlineData("7199999888", false)] // dígitos insuficientes
    [InlineData("", false)] // vazio
    public void IsValidPhone_ShouldReturnExpected(string phone, bool expected) =>
        phone.IsValidTelephone().Should().Be(expected);

    [Fact]
    public void ToTelephoneFormatted_Cellphone_ShouldFormatCorrectly() =>
        "71999998888".ToTelephoneFormatted().Should().Be("(71) 99999-8888");

    [Fact]
    public void ToTelephoneFormatted_Fixed_ShouldFormatCorrectly() =>
        "7133334444".ToTelephoneFormatted().Should().Be("(71) 3333-4444");

    [Fact]
    public void ToTelephoneFormatted_WhenInvalidLength_ShouldReturnEmpty() =>
        "123".ToTelephoneFormatted().Should().BeEmpty();
}