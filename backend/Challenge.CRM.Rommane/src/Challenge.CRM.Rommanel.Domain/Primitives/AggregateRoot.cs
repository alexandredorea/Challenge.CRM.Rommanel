using Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;

namespace Challenge.CRM.Rommanel.Domain.Primitives;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _uncommittedEvents = [];

    public int Version { get; private set; } = -1;

    /// <summary>
    /// Construtor para criação de novos agregados.
    /// </summary>
    protected AggregateRoot() : base()
    {
    }

    /// <summary>
    /// Retorna os eventos ainda não persistidos no Event Store.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents()
        => _uncommittedEvents.AsReadOnly();

    /// <summary>
    /// Limpa a lista de eventos após confirmação de persistência.
    /// </summary>
    public void MarkEventsAsCommitted()
        => _uncommittedEvents.Clear();

    /// <summary>
    /// Aplica um novo evento: atualiza o estado do agregado, incrementa a versão
    /// e adiciona à lista de eventos não commitados.
    /// </summary>
    protected void ApplyChange(IDomainEvent @event)
    {
        Apply(@event);
        Version++;
        var enriched = @event is DomainEvent domainEvent
            ? domainEvent with { Version = Version }
            : @event;
        _uncommittedEvents.Add(enriched);
    }

    /// <summary>
    /// Reconstrói o estado do agregado a partir do histórico de eventos (rehydration).
    /// </summary>
    /// <remarks>
    /// Não adiciona à lista de eventos não commitados — apenas aplica o estado.
    /// </remarks>
    public void LoadsFromHistory(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
        {
            ApplyChange(@event);
            Version++;
        }
    }

    /// <summary>
    /// Despacha o evento para o método Apply correto via pattern matching.
    /// Cada agregado implementa os overloads privados para seus próprios eventos.
    /// </summary>
    protected abstract void Apply(IDomainEvent @event);
}