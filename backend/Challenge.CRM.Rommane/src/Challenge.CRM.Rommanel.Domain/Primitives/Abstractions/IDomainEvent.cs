namespace Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;

/// <summary>
/// Contrato base para todos os eventos de domínio.
/// </summary>
public interface IDomainEvent
{
    Guid Id { get; }
    int Version { get; } //Version permite controle de concorrência.
    DateTime OccurredAt { get; }
    string UserId { get; }
    string CorrelationId { get; } //CorrelationId permite rastrear todos os eventos de uma mesma requisição.
}