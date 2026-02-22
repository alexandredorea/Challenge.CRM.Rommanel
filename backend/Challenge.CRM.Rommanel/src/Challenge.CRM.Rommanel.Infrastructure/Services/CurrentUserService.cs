using Challenge.CRM.Rommanel.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Challenge.CRM.Rommanel.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    public string UserId
        => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
}

public sealed class CorrelationIdProvider(IHttpContextAccessor accessor) : ICorrelationIdProvider
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    public string Value 
        => accessor.HttpContext?.Request.Headers[CorrelationIdHeader].FirstOrDefault()
        ?? Guid.NewGuid().ToString();
}