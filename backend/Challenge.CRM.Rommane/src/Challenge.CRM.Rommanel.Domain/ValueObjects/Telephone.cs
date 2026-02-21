using Challenge.CRM.Rommanel.Domain.Abstractions;
using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Extensions;

namespace Challenge.CRM.Rommanel.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um número de telefone brasileiro com DDD.
/// Armazena internamente apenas os dígitos e expõe a formatação sob demanda.
/// Aceita telefone fixo (10 dígitos) e celular (11 dígitos).
/// </summary>
public sealed class Telephone : ValueObject
{
    //public PhoneType Type { get; }

    public string Number { get; }
    public string FormattedNumber => Number.ToTelephoneFormatted();

    private Telephone(string value) => Number = value;

    /// <summary>
    /// Cria uma instância de Telefone a partir de uma string com ou sem formatação.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException">Lançada quando o telefone é nulo, vazio ou não possui DDD + número válido.</exception>
    public static Telephone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(nameof(Telephone), "Campo obrigatório.");

        var digits = value.OnlyDigits();

        if (!digits.IsValidTelephone())
            throw new DomainException(nameof(Telephone), "O telefone deve conter DDD + número (10 ou 11 dígitos).");

        return new Telephone(digits);
    }

    public override string ToString() => FormattedNumber;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Number;
    }
}