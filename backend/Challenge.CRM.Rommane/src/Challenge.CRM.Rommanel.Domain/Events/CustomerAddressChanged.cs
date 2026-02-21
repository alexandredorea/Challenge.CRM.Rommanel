using Challenge.CRM.Rommanel.Domain.Abstractions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;

namespace Challenge.CRM.Rommanel.Domain.Events;

public sealed record CustomerAddressChanged(
    Address Address,
    string UserId,
    string CorrelationId
) : DomainEvent(UserId, CorrelationId);