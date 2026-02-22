using Challenge.CRM.Rommanel.Domain.Exceptions;
using Challenge.CRM.Rommanel.Domain.Extensions;
using Challenge.CRM.Rommanel.Domain.Primitives;

namespace Challenge.CRM.Rommanel.Domain.ValueObjects;

/// <summary>
/// Value Object que representa uma Unidade Federativa brasileira.
/// Pode ser construído a partir da sigla ("BA") ou do nome do estado ("Bahia").
/// </summary>
public sealed class FederativeUnit : ValueObject
{
    public string Abbreviation { get; }

    public string State => Abbreviation.ToStateName()!;

    private FederativeUnit(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(nameof(FederativeUnit), "Campo obrigatório.");

        var abbreviation = value.NormalizeToFederativeUnit()
            ?? throw new DomainException(nameof(FederativeUnit), $"'{value}' não é uma UF ou estado brasileiro válido.");

        Abbreviation = abbreviation;
    }

    /// <summary>
    ///  Cria uma UF a partir da sigla ou nome do estado.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException">Lançada quando a UF não é reconhecida.</exception>
    /// <returns></returns>
    public static FederativeUnit Create(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Abbreviation;
        //yield return Estate;
    }

    public override string ToString() => Abbreviation;
}