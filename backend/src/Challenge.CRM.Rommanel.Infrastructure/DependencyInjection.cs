using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Infrastructure.ExternalServices.ViaCep;
using Challenge.CRM.Rommanel.Infrastructure.Persistence.Database;
using Challenge.CRM.Rommanel.Infrastructure.Persistence.EventStore;
using Challenge.CRM.Rommanel.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;

namespace Challenge.CRM.Rommanel.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddDatabase();
        builder.AddViaCepExternalServiceApi();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        builder.Services.AddScoped<IEventStore, CustomerEventStore>();
        return builder;
    }

    private static void AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(opt =>
                    opt.UseNpgsql(
                        builder.Configuration.GetConnectionString("Default"),
                        npgsql => npgsql.MigrationsAssembly(
                            typeof(AppDbContext).Assembly.FullName)));
        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    }

    private static void AddViaCepExternalServiceApi(this IHostApplicationBuilder builder)
    {
        builder.Services
                    .AddHttpClient<IViaCepService, ViaCepService>(client =>
                    {
                        client.BaseAddress = new Uri("https://viacep.com.br/ws/");
                        client.Timeout = TimeSpan.FromSeconds(10);
                    })
                    .AddPolicyHandler(RetryPolicy)
                    .AddPolicyHandler(CircuitBreakerPolicy);
    }

    /// <summary>
    /// Retry exponencial: 3 tentativas com espera de 1s, 2s e 4s.
    /// Ativa apenas para erros transientes (5xx, timeout, network).
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> RetryPolicy =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (outcome, timespan, attempt, _) =>
                    Console.WriteLine(
                        $"[Polly] ViaCEP retry {attempt} após {timespan.TotalSeconds}s " +
                        $"— {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}"));

    /// <summary>
    /// Circuit Breaker: abre o circuito após 5 falhas consecutivas por 30s.
    /// Evita cascata de falhas quando o ViaCEP está indisponível.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
}