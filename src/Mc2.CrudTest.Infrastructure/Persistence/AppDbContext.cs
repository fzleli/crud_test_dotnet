using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Mc2.CrudTest.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<DomainEventEntity> DomainEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=cruddb;Username=postgres;Password=pass"); 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(builder =>
        {
            builder.HasIndex(c => new { c.FirstName, c.LastName, c.DateOfBirth })
                   .IsUnique();

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .IsRequired();

                email.HasIndex(e => e.Value).IsUnique();
            });

            builder.OwnsOne(c => c.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                     .HasColumnName("PhoneNumber")
                     .HasColumnType("bigint")
                     .IsRequired();
            });

            builder.OwnsOne(c => c.BankAccountNumber, bank =>
            {
                bank.Property(b => b.Value)
                    .HasColumnName("BankAccountNumber")
                    .IsRequired();
            });
        });

        modelBuilder.Entity<DomainEventEntity>().HasKey(e => e.Id);
        modelBuilder.Ignore<DomainEvent>();
    }
}

