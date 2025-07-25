using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Infrastructure.Data;

/// <summary>
/// EF Core DbContext configured for SQLCipher encryption.
/// </summary>
public class OperationPrimeDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly ILoggerFactory _loggerFactory;

    public OperationPrimeDbContext(string connectionString, ILoggerFactory loggerFactory)
    {
        _connectionString = connectionString;
        _loggerFactory = loggerFactory;
    }

    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite(_connectionString)
            .UseLoggerFactory(_loggerFactory);
    }
}
