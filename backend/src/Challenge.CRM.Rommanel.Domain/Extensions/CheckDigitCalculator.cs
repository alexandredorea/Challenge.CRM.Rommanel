namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Calculadora de dígito verificador configurável via Fluent API.
/// Suporta os algoritmos Módulo 11 utilizados em CPF e CNPJ.
/// </summary>
public sealed class CheckDigitCalculator
{
    private string number;
    private readonly int modulo = 11;
    private List<int> multipliers = [2, 3, 4, 5, 6, 7, 8, 9];
    private readonly Dictionary<int, string> replacements = [];
    private readonly bool useComplement = true;

    public CheckDigitCalculator(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        this.number = number;
    }

    /// <summary>
    /// Define multiplicadores como intervalo contínuo crescente ou decrescente.
    /// Ex: WithMultipliers(2, 9) → [2,3,4,5,6,7,8,9]
    ///     WithMultipliers(9, 2) → [9,8,7,6,5,4,3,2]
    /// </summary>
    public CheckDigitCalculator WithMultipliers(int start, int end)
    {
        if (start > end)
            multipliers = Enumerable.Range(end, start - end + 1).Reverse().ToList();
        else
        {
            multipliers = new List<int>();

            for (int i = start; i >= end; i--)
                multipliers.Add(i);
        }
        return this;
    }

    /// <summary>
    /// Define multiplicadores explicitamente na ordem informada.
    /// Ex: WithMultipliers(5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2)
    /// </summary>
    public CheckDigitCalculator WithMultipliers(params int[] multiplicators)
    {
        if (multiplicators is null || multiplicators.Length == 0)
            throw new ArgumentException(
                "Ao menos um multiplicador deve ser informado.", nameof(multipliers));

        multipliers = multiplicators.ToList();
        return this;
    }

    /// <summary>
    /// Substitui resultados específicos do cálculo por uma string.
    /// Ex: ReplaceWith("0", 10, 11) → resultado 10 ou 11 vira "0".
    /// </summary>
    public CheckDigitCalculator ReplaceWith(string replacement, params int[] digits)
    {
        foreach (var digit in digits)
        {
            replacements[digit] = replacement;
        }
        return this;
    }

    /// <summary>
    /// Acrescenta um dígito já calculado ao número base para recalcular o próximo dígito.
    /// </summary>
    public CheckDigitCalculator AddDigit(string digit)
    {
        number = string.Concat(number, digit);
        return this;
    }

    /// <summary>
    /// Executa o cálculo do dígito verificador com base na configuração atual.
    /// </summary>
    public string Calculate()
    {
        if (string.IsNullOrEmpty(number))
            return string.Empty;

        var sum = number
            .Select((digit, index) => char.GetNumericValue(digit) * multipliers[index % multipliers.Count])
            .Sum();

        var modResult = sum % modulo;
        var result = useComplement ? modulo - modResult : modResult;

        if (result >= 10)
            result = 0;

        return replacements.ContainsKey((int)result) ? replacements[(int)result] : result.ToString();
    }
}