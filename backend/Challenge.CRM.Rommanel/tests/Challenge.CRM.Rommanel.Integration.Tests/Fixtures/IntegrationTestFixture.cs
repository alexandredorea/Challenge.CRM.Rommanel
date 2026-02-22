using Challenge.CRM.Rommanel.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Challenge.CRM.Rommanel.Integration.Tests.Fixtures;

// <summary>
/// Fixture compartilhada entre todos os testes de integração.
/// Sobe um container PostgreSQL real via Testcontainers.
/// WebApplicationFactory reconfigura o DbContext para usar este banco.
/// </summary>
public sealed class IntegrationTestFixture
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("teste")
        .WithDatabase("crm_test")
        .WithUsername("crm")
        .WithPassword("crm@test")
        .Build();

    public HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
        Client = CreateClient();

        // Aplica migrations no banco de teste
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext original
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Adiciona DbContext apontando para o container de teste
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(_postgres.GetConnectionString()));
        });

        // Desabilita autenticação nos testes de integração
        builder.UseEnvironment("Testing");
    }
}