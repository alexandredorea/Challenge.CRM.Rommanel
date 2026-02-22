using Challenge.CRM.Rommanel.Domain.Extensions;
using Challenge.CRM.Rommanel.Infrastructure.Persistence.EventStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenge.CRM.Rommanel.Infrastructure.Persistence.Configurations;

public sealed class CustomerEventConfiguration : IEntityTypeConfiguration<StoredEvent>
{
    public void Configure(EntityTypeBuilder<StoredEvent> builder)
    {
        builder.ToTable($"customer_events");

        builder.HasKey(c => c.Id)
            .HasName($"pk_customer_events_id");

        builder.Property(c => c.Id)
            .HasColumnName($"{nameof(StoredEvent.Id)}".ToSnakeCase())
            .ValueGeneratedOnAdd();

        builder.Property(e => e.AggregateId)
            .HasColumnName($"{nameof(StoredEvent.AggregateId)}".ToSnakeCase())
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasColumnName($"{nameof(StoredEvent.EventType)}".ToSnakeCase())
            .HasMaxLength(100)
            .IsRequired();

        // Payload serializado como JSONB no PostgreSQL
        builder.Property(e => e.Payload)
            .HasColumnName($"{nameof(StoredEvent.Payload)}".ToSnakeCase())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.Version)
            .HasColumnName($"{nameof(StoredEvent.Version)}".ToSnakeCase())
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasColumnName($"{nameof(StoredEvent.UserId)}".ToSnakeCase())
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.CorrelationId)
            .HasColumnName($"{nameof(StoredEvent.CorrelationId)}".ToSnakeCase())
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.OccurredAt)
            .HasColumnName($"{nameof(StoredEvent.OccurredAt)}".ToSnakeCase())
            .IsRequired();

        builder.HasIndex(e => e.AggregateId)
            .HasDatabaseName("ix_customer_events_aggregate_id");

        builder.HasIndex(e => new { e.AggregateId, e.Version })
            .IsUnique()
            .HasDatabaseName("ix_customer_events_aggregate_version");
    }
}