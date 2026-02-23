using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Challenge.CRM.Rommanel.Integration.Tests.Fixtures
{
    public sealed class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Simula um usuário autenticado em todos os testes
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name,           "Test User"),
                new Claim("preferred_username",      "test.user"),
                new Claim(ClaimTypes.Email,          "test@test.com")
            };

            var identity = new ClaimsIdentity(claims, "Testing");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Testing");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
