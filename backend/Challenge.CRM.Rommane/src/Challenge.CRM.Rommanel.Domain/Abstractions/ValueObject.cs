namespace Challenge.CRM.Rommanel.Domain.Abstractions;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        // GetType() garante que tipos distintos nunca são iguais,
        // mesmo que seus componentes de igualdade coincidam
        if (obj is null || obj.GetType() != GetType())
            return false;

        return ((ValueObject)obj)
            .GetEqualityComponents()
            .SequenceEqual(GetEqualityComponents());
    }

    public bool Equals(ValueObject? other)
        => Equals((object?)other);

    public override int GetHashCode()
        => GetEqualityComponents()
            .Aggregate(0, (hash, comp) =>
                HashCode.Combine(hash, comp?.GetHashCode() ?? 0));

    // Sem esses operadores, == usaria igualdade por referência = errado para VOs
    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !(left == right);
}