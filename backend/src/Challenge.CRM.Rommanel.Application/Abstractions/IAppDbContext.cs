using Challenge.CRM.Rommanel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challenge.CRM.Rommanel.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}