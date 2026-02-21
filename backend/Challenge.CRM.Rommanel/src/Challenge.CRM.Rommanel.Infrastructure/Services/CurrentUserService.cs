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
    public string Value
        => accessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault()
        ?? Guid.NewGuid().ToString();
}