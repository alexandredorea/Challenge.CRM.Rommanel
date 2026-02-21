namespace Challenge.CRM.Rommanel.Domain.Extensions;

/// <summary>
/// Calculadora de dígito verificador configurável via Fluent API.
/// Suporta os algoritmos Módulo 11 utilizados em CPF e CNPJ.
/// </summary>
public sealed class CheckDigitCalculator
{
    private string _number;
    private readonly int _modulo = 11;
    private readonly bool _useComplement = true;
    private List<int> _multipliers = [2, 3, 4, 5, 6, 7, 8, 9];
    private readonly Dictionary<int, string> _replacements = [];

    public CheckDigitCalculator(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        _number = number;
    }

    /// <summary>
    /// Define multiplicadores como intervalo contínuo crescente ou decrescente.
    /// Ex: WithMultipliers(2, 9) → [2,3,4,5,6,7,8,9]
    ///     WithMultipliers(9, 2) → [9,8,7,6,5,4,3,2]
    /// </summary>
    public CheckDigitCalculator WithMultipliers(int start, int end)
    {
        _multipliers = start <= end
            ? Enumerable.Range(start, end - start + 1).ToList()
            : Enumerable.Range(end, start - end + 1).Reverse().ToList();

        return this;
    }

    /// <summary>
    /// Define multiplicadores explicitamente na ordem informada.
    /// Ex: WithMultipliers(5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2)
    /// </summary>
    public CheckDigitCalculator WithMultipliers(params int[] multipliers)
    {
        if (multipliers is null || multipliers.Length == 0)
            throw new ArgumentException(
                "Ao menos um multiplicador deve ser informado.", nameof(multipliers));

        _multipliers = multipliers.ToList();
        return this;
    }

    /// <summary>
    /// Substitui resultados específicos do cálculo por uma string.
    /// Ex: ReplaceWith("0", 10, 11) → resultado 10 ou 11 vira "0".
    /// </summary>
    public CheckDigitCalculator ReplaceWith(string replacement, params int[] results)
    {
        foreach (var result in results)
            _replacements[result] = replacement;

        return this;
    }

    /// <summary>
    /// Acrescenta um dígito já calculado ao número base para recalcular o próximo dígito.
    /// </summary>
    public CheckDigitCalculator AddDigit(string digit)
    {
        _number = string.Concat(_number, digit);
        return this;
    }

    /// <summary>
    /// Executa o cálculo do dígito verificador com base na configuração atual.
    /// </summary>
    public string Calculate()
    {
        if (string.IsNullOrEmpty(_number))
            return string.Empty;

        var sum = 0;

        // Itera da direita para esquerda — exigência dos algoritmos CPF/CNPJ
        for (int i = _number.Length - 1, m = 0; i >= 0; i--, m++)
        {
            var digit = (int)char.GetNumericValue(_number[i]);
            sum += digit * _multipliers[m % _multipliers.Count];
        }

        var modResult = sum % _modulo;
        var result = _useComplement ? _modulo - modResult : modResult;

        return _replacements.TryGetValue(result, out var replacement)
            ? replacement
            : result.ToString();
    }
}