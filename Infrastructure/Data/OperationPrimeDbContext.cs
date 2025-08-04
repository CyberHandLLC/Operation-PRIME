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
    /// Gets or sets the incidents in the database.
    /// </summary>
    public DbSet<Incident> Incidents { get; set; }
    
    /// <summary>
    /// Gets or sets the applications in the database.
    /// Used for auto-suggestion in incident creation.
    /// </summary>
    public DbSet<ApplicationInfo> Applications { get; set; }

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
            // Fallback connection string if not configured via DI
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
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.BusinessImpact).HasMaxLength(500);
            // ImpactedUsers is now an integer, no MaxLength needed
            entity.Property(e => e.ApplicationAffected).HasMaxLength(200);
            entity.Property(e => e.LocationsAffected).HasMaxLength(300);
            entity.Property(e => e.Workaround).HasMaxLength(500);
            entity.Property(e => e.IncidentNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).IsRequired();
        });
        
        // Configure ApplicationInfo entity
        modelBuilder.Entity<ApplicationInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).IsRequired();
            
            // Create index for faster searches during auto-suggestion
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });
    }
}