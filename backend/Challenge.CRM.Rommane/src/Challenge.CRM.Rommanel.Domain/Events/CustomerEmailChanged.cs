using Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;

namespace Challenge.CRM.Rommanel.Domain.Events;

public sealed record CustomerEmailChanged(
    Email Email,
    string UserId,
    string CorrelationId
) : DomainEvent(UserId, CorrelationId);