using Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;

namespace Challenge.CRM.Rommanel.Application.Abstractions;

/// <summary>
/// Contrato para persistência append-only dos eventos de domínio.
/// A implementação grava na tabela customer_events dentro da mesma
/// transação do IUnitOfWork — sem dual-write.
/// </summary>
public interface IEventStore
{
    Task AppendAsync(
        Guid aggregateId,
        IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IDomainEvent>> LoadAsync(
        Guid aggregateId,
        CancellationToken cancellationToken = default);
}