using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OperationPrime.Infrastructure.Options;

namespace OperationPrime.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating <see cref="OperationPrimeDbContext"/>.
/// Enables EF Core migrations without requiring the main application host.
/// </summary>
public class OperationPrimeDbContextFactory : IDesignTimeDbContextFactory<OperationPrimeDbContext>
{
    /// <inheritdoc />
    public OperationPrimeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = configuration.GetSection("Database").Get<DatabaseOptions>()
            ?? new DatabaseOptions
            {
                ConnectionString = "Data Source=incidents.db",
                EncryptionKey = "local-dev-key"
            };

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        return new OperationPrimeDbContext(
            Microsoft.Extensions.Options.Options.Create(options),
            loggerFactory);
    }
}

