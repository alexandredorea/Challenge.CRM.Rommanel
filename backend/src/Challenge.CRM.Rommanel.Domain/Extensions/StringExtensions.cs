using System.Globalization;
using System.Text;

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

    /// <summary>
    /// Converte uma string em formato PascalCase ou camelCase para snake_case.
    /// Algoritmo baseado na implementação oficial do EFCore.NamingConventions:
    /// https://github.com/efcore/EFCore.NamingConventions/blob/main/EFCore.NamingConventions/Internal/SnakeCaseNameRewriter.cs
    /// </summary>
    /// <param name="input">String de entrada no formato PascalCase ou camelCase.</param>
    /// <param name="culture">
    /// Cultura usada para conversão para minúsculas.
    /// Padrão: <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <returns>
    /// String convertida para snake_case, ou o valor original se nulo ou vazio.
    /// </returns>
    /// <example>
    /// "CustomerName"  → "customer_name"
    /// "parseHTTPCall" → "parse_http_call"
    /// "MyHTTPSApi"    → "my_https_api"
    /// </example>
    public static string ToSnakeCase(this string input, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var cultureInfo = culture ?? CultureInfo.InvariantCulture;
        var builder = new StringBuilder(input.Length + Math.Min(2, input.Length / 5));
        var previousCategory = default(UnicodeCategory?);

        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];
            var currentCategory = char.GetUnicodeCategory(currentChar);

            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                        previousCategory == UnicodeCategory.LowercaseLetter ||
                        previousCategory != UnicodeCategory.DecimalDigitNumber &&
                        previousCategory != null &&
                        i > 0 &&
                        i + 1 < input.Length &&
                        char.IsLower(input[i + 1]))
                    {
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar, cultureInfo);
                    break;

                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                        builder.Append('_');
                    break;

                default:
                    if (previousCategory != null)
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }
}