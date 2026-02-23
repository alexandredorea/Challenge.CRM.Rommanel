using Challenge.CRM.Rommanel.Application.Abstractions;
using Challenge.CRM.Rommanel.Domain.Entities;
using Challenge.CRM.Rommanel.Infrastructure.Persistence.EventStore;
using Microsoft.EntityFrameworkCore;

namespace Challenge.CRM.Rommanel.Infrastructure.Persistence.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<StoredEvent> CustomerEvents => Set<StoredEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}