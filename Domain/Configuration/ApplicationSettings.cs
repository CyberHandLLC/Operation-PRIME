namespace OperationPrime.Domain.Configuration;

/// <summary>
/// Application configuration settings following Clean Architecture principles.
/// Maps to appsettings.json ApplicationSettings section.
/// </summary>
public class ApplicationSettings
{

    
    /// <summary>
    /// Default time zone for business operations.
    /// </summary>
    public string DefaultTimeZone { get; set; } = "Eastern Standard Time";
    
    /// <summary>
    /// Maximum retry attempts for operations.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
    
    /// <summary>
    /// Whether database migrations should be applied automatically.
    /// </summary>
    public bool DatabaseMigrationEnabled { get; set; } = true;
}

/// <summary>
/// Seed data configuration for applications.
/// Maps to appsettings.json SeedData section.
/// </summary>
public class SeedDataSettings
{
    /// <summary>
    /// List of common applications to seed in the database.
    /// </summary>
    public List<ApplicationSeedData> CommonApplications { get; set; } = new();
}

/// <summary>
/// Application seed data structure.
/// </summary>
public class ApplicationSeedData
{
    /// <summary>
    /// Application name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Application description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}