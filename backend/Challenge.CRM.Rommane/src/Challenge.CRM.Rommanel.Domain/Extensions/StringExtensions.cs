namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Métodos de extensão para operações comuns em strings.
/// </summary>
public static class stringExtensions
{
    /// <summary>
    /// Remove todos os caracteres não numéricos da string.
    /// </summary>
    public static string OnlyDigits(this string value) =>
        new string(value.Where(char.IsDigit).ToArray());
}