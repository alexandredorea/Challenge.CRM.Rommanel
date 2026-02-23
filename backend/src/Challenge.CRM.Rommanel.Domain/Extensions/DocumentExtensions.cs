namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Métodos de extensão para operações de formatação e validação de CPF e CNPJ.
/// </summary>
public static class DocumentExtensions
{
    /// <summary>
    /// Retorna true se a string for um CPF válido (11 dígitos, dígitos verificadores corretos).
    /// </summary>
    public static bool IsValidCpf(this string value)
    {
        var digits = value.OnlyDigits();
        if (digits.Length != 11 || digits.Distinct().Count() == 1)
            return false;

        var number = digits[..9];
        var verifier = new CheckDigitCalculator(number)
            .WithMultipliers(10, 2)
            .ReplaceWith("0", 10, 11);

        var first = verifier.Calculate();
        var second = verifier
            .AddDigit(first)
            .WithMultipliers(11, 2)
            .Calculate();

        return $"{first}{second}" == digits[9..];
    }

    /// <summary>
    /// Retorna true se a string for um CNPJ válido (14 dígitos, dígitos verificadores corretos).
    /// </summary>
    public static bool IsValidCnpj(this string value)
    {
        var digits = value.OnlyDigits();
        if (digits.Length != 14 || digits.Distinct().Count() == 1)
            return false;

        var number = digits[..12];
        var verifier = new CheckDigitCalculator(number)
            .WithMultipliers(5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2)
            .ReplaceWith("0", 10, 11);

        var first = verifier.Calculate();
        var second = verifier
            .AddDigit(first)
            .WithMultipliers(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2)
            .Calculate();

        return $"{first}{second}" == digits[12..];
    }

    /// <summary>
    /// Retorna o CPF formatado com máscara (ex: "529.982.247-25").
    /// </summary>
    public static string ToCpfFormatted(this string digits) =>
        digits.Length == 11
            ? $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}"
            : string.Empty;

    /// <summary>
    /// Retorna o CNPJ formatado com máscara (ex: "11.222.333/0001-81").
    /// </summary>
    public static string ToCnpjFormatted(this string digits) =>
        digits.Length == 14
            ? $"{digits[..2]}.{digits[2..5]}.{digits[5..8]}/{digits[8..12]}-{digits[12..]}"
            : string.Empty;
}