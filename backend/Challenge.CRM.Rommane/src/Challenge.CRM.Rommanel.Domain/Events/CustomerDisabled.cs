using Challenge.CRM.Rommanel.Domain.Abstractions;

namespace Challenge.CRM.Rommanel.Domain.Events;

public sealed record CustomerDisabled(
    string UserId,
    string CorrelationId
) : DomainEvent(UserId, CorrelationId);