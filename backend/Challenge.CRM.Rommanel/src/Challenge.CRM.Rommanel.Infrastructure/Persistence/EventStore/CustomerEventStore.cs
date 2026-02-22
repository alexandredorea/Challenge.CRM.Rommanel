using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Domain.Events;
using Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;
using Challenge.CRM.Rommanel.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Challenge.CRM.Rommanel.Infrastructure.Persistence.EventStore;

/// <summary>
/// Implementação do Event Store usando a tabela customer_events no PostgreSQL.
/// Serializa/deserializa os eventos como JSONB.
/// A gravação ocorre dentro da mesma transação do IAppDbContext.
/// </summary>
public sealed class CustomerEventStore(AppDbContext context) : IEventStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // Mapa de tipos conhecidos para deserialização
    private static readonly IReadOnlyDictionary<string, Type> EventTypes =
        new Dictionary<string, Type>
        {
            { nameof(CustomerCreated),                  typeof(CustomerCreated) },
            { nameof(CustomerEmailChanged),             typeof(CustomerEmailChanged) },
            { nameof(CustomerTelephoneChanged),         typeof(CustomerTelephoneChanged) },
            { nameof(CustomerAddressChanged),           typeof(CustomerAddressChanged) },
            { nameof(CustomerDisabled),                 typeof(CustomerDisabled) }
        };

    public async Task AppendAsync(
        Guid aggregateId,
        IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        var storedEvents = events.Select(e => new StoredEvent
        {
            Id = e.Id,
            AggregateId = aggregateId,
            EventType = e.GetType().Name,
            Payload = JsonSerializer.Serialize(e, e.GetType(), JsonOptions),
            Version = e.Version,
            UserId = e.UserId,
            CorrelationId = e.CorrelationId,
            OccurredAt = e.OccurredAt
        });

        await context.CustomerEvents.AddRangeAsync(storedEvents, cancellationToken);

        // Não chama SaveChanges aqui —
        // a persistência é responsabilidade do IAppDbContext (SaveChangesAsync)
    }

    public async Task<IReadOnlyList<IDomainEvent>> LoadAsync(
        Guid aggregateId,
        CancellationToken cancellationToken = default)
    {
        var storedEvents = await context.CustomerEvents
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var domainEvents = new List<IDomainEvent>();

        foreach (var stored in storedEvents)
        {
            if (!EventTypes.TryGetValue(stored.EventType, out var type))
                continue;

            var domainEvent = JsonSerializer.Deserialize(stored.Payload, type, JsonOptions)
                as IDomainEvent;

            if (domainEvent is not null)
                domainEvents.Add(domainEvent);
        }

        return domainEvents;
    }
}