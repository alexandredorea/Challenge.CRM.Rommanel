namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Métodos de extensão para limpeza, formatação e validação de CEP brasileiro.
/// </summary>
public static class PostalCodeExtensions
{
    private const int CepLength = 8;

    /// <summary>
    /// Verifica se a string representa um CEP válido (8 dígitos, faixa 01000-000 a 99999-999).
    /// </summary>
    public static bool IsValidPostalCode(this string value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.OnlyDigits().Length == CepLength &&
        value.IsValidPostalCodeRange();

    /// <summary>
    /// Verifica se os dígitos do CEP estão dentro da faixa válida do Brasil.
    /// CEPs válidos: 01000-000 (1000000) até 99999-999 (99999999).
    /// CEPs zerados (00000-000) são inválidos por definição dos Correios.
    /// </summary>
    public static bool IsValidPostalCodeRange(this string value)
    {
        var digits = value.OnlyDigits();

        if (digits.Length != CepLength)
            return false;

        // CEP "00000000" é inválido — todos zeros não corresponde a nenhuma faixa real
        if (digits.Distinct().Count() == 1 && digits[0] == '0')
            return false;

        // o limite mínimo 1_000_000 representa corretamente o CEP "01000000"
        return int.TryParse(digits, out var number) && number >= 1_000_000;
    }

    /// <summary>
    /// Retorna o CEP formatado com máscara (ex: "12345-678").
    /// Retorna string vazia se o input não tiver exatamente 8 dígitos.
    /// </summary>
    public static string ToPostalCodeFormatted(this string digits) =>
        digits.Length == CepLength
            ? $"{digits[..5]}-{digits[5..]}"
            : string.Empty;
}