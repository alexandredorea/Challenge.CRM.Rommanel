namespace Challenge.CRM.Rommanel.Application.DTOs;

public sealed record CustomerEventDto(
    Guid EventId,
    string EventType,
    int Version,
    string UserId,
    string CorrelationId,
    DateTime OccurredAt,
    object Payload
);