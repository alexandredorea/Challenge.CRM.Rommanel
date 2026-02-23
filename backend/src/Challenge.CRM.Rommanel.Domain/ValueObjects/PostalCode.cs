using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Extensions;
using Challenge.CRM.Rommanel.Domain.Primitives;

namespace Challenge.CRM.Rommanel.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um Código de Endereçamento Postal (CEP) brasileiro.
/// Armazena internamente apenas os dígitos e expõe a formatação sob demanda.
/// </summary>
public sealed class PostalCode : ValueObject
{
    public string Number { get; } = string.Empty;

    public string Formatted => Number.ToPostalCodeFormatted();

    private PostalCode()
    { }

    private PostalCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(nameof(PostalCode), "Campo obrigatório.");

        var digits = value.OnlyDigits();

        if (!digits.IsValidPostalCode())
            throw new DomainException(nameof(PostalCode), $"'{value}' não é um CEP válido. Informe 8 dígitos numéricos.");

        Number = digits;
    }

    /// <summary>
    /// Cria um CEP validado a partir de uma string com ou sem formatação.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException">Lançada quando o CEP informado não possui 8 dígitos válidos.</exception>
    /// <returns></returns>
    public static PostalCode Create(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Number;
    }

    public override string ToString() => Formatted;
}
