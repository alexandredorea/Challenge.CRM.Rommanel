namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Métodos de extensão para validação e formatação de telefone brasileiro.
/// Aplica as regras da ANATEL: DDD válido, dígito inicial e faixa de numeração.
/// </summary>
public static class TelephoneExtensions
{
    /// <summary>
    /// DDDs oficialmente ativos no Brasil conforme ANATEL.
    /// </summary>
    private static readonly HashSet<string> ValidDdd = new(stringComparer.Ordinal)
    {
        // Sudeste
        "11", "12", "13", "14", "15", "16", "17", "18", "19", // SP
        "21", "22", "24",                                     // RJ
        "27", "28",                                           // ES
        "31", "32", "33", "34", "35", "37", "38",             // MG
        // Sul
        "41", "42", "43", "44", "45", "46",                   // PR
        "47", "48", "49",                                     // SC
        "51", "53", "54", "55",                               // RS
        // Centro-Oeste
        "61",                                                 // DF
        "62", "64",                                           // GO
        "63",                                                 // TO
        "65", "66",                                           // MT
        "67",                                                 // MS
        // Nordeste
        "68",                                                 // AC
        "69",                                                 // RO
        "71", "73", "74", "75", "77",                         // BA
        "79",                                                 // SE
        "81", "87",                                           // PE
        "82",                                                 // AL
        "83",                                                 // PB
        "84",                                                 // RN
        "85", "88",                                           // CE
        "86", "89",                                           // PI
        "91", "93", "94",                                     // PA
        "92", "97",                                           // AM
        "95",                                                 // RR
        "96",                                                 // AP
        "98", "99",                                           // MA
    };

    /// <summary>
    /// Dígitos iniciais válidos para telefone fixo (STFC).
    /// Conforme ANATEL: 2, 3, 4 e 5.
    /// </summary>
    private static readonly HashSet<char> InitialDigitsFixed = ['2', '3', '4', '5'];

    /// <summary>
    /// Verifica se a string representa um telefone válido aplicando
    /// as regras da ANATEL: DDD ativo, comprimento, dígito inicial e faixa de numeração.
    /// Aceita com ou sem formatação (ex: "71999998888" ou "(71) 99999-8888").
    /// </summary>
    public static bool IsValidTelephone(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var digits = value.OnlyDigits();

        return digits.Length switch
        {
            11 => IsValidCellphone(digits),
            10 => IsValidLandline(digits),
            _ => false
        };
    }

    /// <summary>
    /// Retorna o telefone formatado com máscara.
    /// Celular (11 dígitos): "(71) 99999-8888"
    /// Fixo   (10 dígitos): "(71) 3333-8888"
    /// Retorna string vazia se inválido.
    /// </summary>
    public static string ToTelephoneFormatted(this string digits)
        => digits.Length switch
        {
            11 => $"({digits[..2]}) {digits[2..7]}-{digits[7..]}",
            10 => $"({digits[..2]}) {digits[2..6]}-{digits[6..]}",
            _ => string.Empty
        };

    private static bool IsValidCellphone(string digits)
    {
        var ddd = digits[..2];
        var firstDigit = digits[2];  // deve ser '9'
        var secondDigit = digits[3]; // deve ser 6, 7, 8 ou 9

        return ValidDdd.Contains(ddd)
            && firstDigit == '9'
            && secondDigit is >= '6' and <= '9';
    }

    private static bool IsValidLandline(string digits)
    {
        var ddd = digits[..2];
        var firstDigit = digits[2];    // deve ser 2, 3, 4 ou 5

        return ValidDdd.Contains(ddd)
            && InitialDigitsFixed.Contains(firstDigit);
    }
}