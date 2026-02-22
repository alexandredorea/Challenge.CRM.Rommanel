namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Métodos de extensão para operações comuns em strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Remove todos os caracteres não numéricos da string.
    /// </summary>
    public static string OnlyDigits(this string value)
        => new(value.Where(char.IsDigit).ToArray());
}