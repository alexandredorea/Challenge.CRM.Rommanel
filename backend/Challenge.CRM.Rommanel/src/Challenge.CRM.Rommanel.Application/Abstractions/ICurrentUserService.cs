namespace Challenge.CRM.Rommanel.Application.Abstractions;

public interface ICurrentUserService
{
    string UserId { get; }
}

public interface ICorrelationIdProvider
{
    string Value { get; }
}