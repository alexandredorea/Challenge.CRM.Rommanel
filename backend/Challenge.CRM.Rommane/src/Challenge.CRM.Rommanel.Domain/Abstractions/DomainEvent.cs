namespace Challenge.CRM.Rommanel.Domain.Abstractions;

/// <summary>
/// Implementação base imutável para todos os eventos de domínio.
/// Ou seja, para garantir imutabilidade e igualdade por valor.
/// </summary>
public abstract record DomainEvent(
    string UserId,
    string CorrelationId) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Version { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}