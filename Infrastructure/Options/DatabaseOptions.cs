namespace OperationPrime.Infrastructure.Options;

/// <summary>
/// Configuration options for the database connection.
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// Gets or sets the base SQLite connection string.
    /// </summary>
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the encryption key used by SQLCipher.
    /// </summary>
    public required string EncryptionKey { get; set; }
}
