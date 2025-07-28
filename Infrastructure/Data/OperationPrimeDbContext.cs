using Microsoft.EntityFrameworkCore;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for Operation Prime.
/// Configured for SQLite with simple local file storage.
/// </summary>
public class OperationPrimeDbContext : DbContext
{
    /// <summary>
    /// Incidents table for storing all incident data.
    /// </summary>
    public DbSet<Incident> Incidents { get; set; }

    /// <summary>
    /// Initializes a new instance of the OperationPrimeDbContext.
    /// </summary>
    /// <param name="options">Database context options.</param>
    public OperationPrimeDbContext(DbContextOptions<OperationPrimeDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the database context options.
    /// </summary>
    /// <param name="optionsBuilder">Options builder for configuration.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Simple local SQLite database file
            optionsBuilder.UseSqlite("Data Source=incidents.db");
        }
    }

    /// <summary>
    /// Configures entity models and relationships.
    /// </summary>
    /// <param name="modelBuilder">Model builder for configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Incident entity
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).IsRequired();
        });
    }
}
