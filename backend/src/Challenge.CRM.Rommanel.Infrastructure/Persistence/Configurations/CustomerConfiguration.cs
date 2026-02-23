using Challenge.CRM.Rommanel.Domain.Entities;
using Challenge.CRM.Rommanel.Domain.Enumerators;
using Challenge.CRM.Rommanel.Domain.Extensions;
using Challenge.CRM.Rommanel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenge.CRM.Rommanel.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable($"{nameof(Customer)}s".ToSnakeCase());

        builder.HasKey(c => c.Id)
            .HasName($"Pk{nameof(Customer)}s{nameof(Customer.Id)}".ToSnakeCase());

        builder.Property(c => c.Id)
            .HasColumnName($"{nameof(Customer.Id)}".ToSnakeCase())
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasColumnName($"{nameof(Customer.Name)}".ToSnakeCase())
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.OriginDate)
            .HasColumnName($"{nameof(Customer.OriginDate)}".ToSnakeCase())
            .IsRequired();

        builder.Property(c => c.StateRegistration)
            .HasColumnName($"{nameof(Customer.StateRegistration)}".ToSnakeCase())
            .HasMaxLength(30);

        builder.Property(c => c.Active)
            .HasColumnName($"{nameof(Customer.Active)}".ToSnakeCase())
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName($"{nameof(Customer.CreatedAt)}".ToSnakeCase())
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        ValueObjectDocument(builder);

        ValueObjectEmail(builder);

        ValueObjectTelephone(builder);

        ValueObjectAddress(builder);
    }

    private static void ValueObjectAddress(EntityTypeBuilder<Customer> builder)
    {
        builder.OwnsOne(c => c.Address, address =>
        {
            address.OwnsOne(a => a.PostalCode, pc =>
            {
                pc.Property(p => p.Number)
                    .HasColumnName($"{nameof(PostalCode)}".ToSnakeCase())
                    .HasMaxLength(8)
                    .IsRequired();
            });

            address.Property(a => a.Street)
                .HasColumnName($"{nameof(Address.Street)}".ToSnakeCase())
                .HasMaxLength(255)
                .IsRequired();

            address.Property(a => a.Number)
                .HasColumnName($"{nameof(Address)}{nameof(Address.Number)}".ToSnakeCase())
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.Neighborhood)
                .HasColumnName($"{nameof(Address.Neighborhood)}".ToSnakeCase())
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName($"{nameof(Address.City)}".ToSnakeCase())
                .HasMaxLength(100)
                .IsRequired();

            address.OwnsOne(a => a.FederativeUnit, fu =>
            {
                fu.Property(f => f.Abbreviation)
                    .HasColumnName($"{nameof(FederativeUnit)}".ToSnakeCase())
                    .HasMaxLength(2)
                    .IsRequired();
            });
        });
    }

    private static void ValueObjectTelephone(EntityTypeBuilder<Customer> builder)
    {
        builder.OwnsOne(c => c.Telephone, tel =>
        {
            tel.Property(t => t.Number)
                .HasColumnName($"{nameof(Telephone)}".ToSnakeCase())
                .HasMaxLength(11)
                .IsRequired();
        });
    }

    private static void ValueObjectEmail(EntityTypeBuilder<Customer> builder)
    {
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Address)
                .HasColumnName($"{nameof(Email)}".ToSnakeCase())
                .HasMaxLength(254)
                .IsRequired();

            email.HasIndex(e => e.Address)
                .IsUnique()
                .HasDatabaseName($"Uq{nameof(Customer)}s{nameof(Email)}".ToSnakeCase());
        });
    }

    private static void ValueObjectDocument(EntityTypeBuilder<Customer> builder)
    {
        builder.OwnsOne(c => c.Document, doc =>
        {
            doc.Property(d => d.Number)
                .HasColumnName($"{nameof(Document)}{nameof(Document.Number)}".ToSnakeCase())
                .HasMaxLength(14)
                .IsRequired();

            doc.Property(d => d.Type)
                .HasColumnName($"{nameof(Document)}{nameof(Document.Type)}".ToSnakeCase())
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<DocumentType>(v))
                .HasMaxLength(15)
                .IsRequired();

            doc.HasIndex(d => d.Number)
                .IsUnique()
                .HasDatabaseName($"Uq{nameof(Customer)}s{nameof(Document)}{nameof(Document.Number)}".ToSnakeCase());
        });
    }
}