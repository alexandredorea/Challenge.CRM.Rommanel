using Challenge.CRM.Rommanel.Domain.Entities;
using Challenge.CRM.Rommanel.Domain.Primitives.Abstractions;

namespace Challenge.CRM.Rommanel.Domain.Events;

/// <summary>
/// Evento disparado quando um novo cliente é cadastrado.
/// </summary>
public sealed record CustomerCreated(
    Customer Customer,
    string UserId,
    string CorrelationId
) : DomainEvent(UserId, CorrelationId);