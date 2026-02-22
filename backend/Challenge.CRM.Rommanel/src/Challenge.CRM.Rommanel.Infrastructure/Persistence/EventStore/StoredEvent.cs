namespace Challenge.CRM.Rommanel.Infrastructure.Persistence.EventStore;

/// <summary>
/// Representa um evento serializado na tabela customer_events.
/// É o modelo de persistência — não é uma entidade de domínio.
/// </summary>
public sealed class StoredEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AggregateId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty; // JSON
    public int Version { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string CorrelationId { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}