using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OperationPrime.Domain.Entities;
using OperationPrime.Infrastructure.Options;

namespace OperationPrime.Infrastructure.Data;

/// <summary>
/// EF Core DbContext configured for SQLCipher encryption.
/// </summary>
public class OperationPrimeDbContext : DbContext
{
    private readonly DatabaseOptions _dbOptions;
    private readonly ILoggerFactory _loggerFactory;

    public OperationPrimeDbContext(IOptions<DatabaseOptions> options, ILoggerFactory loggerFactory)
    {
        _dbOptions = options.Value;
        _loggerFactory = loggerFactory;
    }

    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<PreIncident> PreIncidents => Set<PreIncident>();
    public DbSet<MajorIncident> MajorIncidents => Set<MajorIncident>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = $"{_dbOptions.ConnectionString};Password={_dbOptions.EncryptionKey}";
        optionsBuilder
            .UseSqlite(connection)
            .UseLoggerFactory(_loggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLogEntry>()
            .HasIndex(e => new { e.IncidentId, e.Timestamp })
            .HasDatabaseName("IX_AuditLog_IncidentId_Timestamp");

        base.OnModelCreating(modelBuilder);
    }
}
