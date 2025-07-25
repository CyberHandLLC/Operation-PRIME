namespace OperationPrime.Domain.Enums;

/// <summary>
/// Defines the current status of an incident throughout its lifecycle.
/// Status progression: New → InProgress → Resolved → Closed
/// </summary>
public enum IncidentStatus
{
    /// <summary>
    /// Incident has been created but not yet assigned or started
    /// </summary>
    New = 1,

    /// <summary>
    /// Incident is actively being worked on
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Incident has been resolved but may need verification
    /// </summary>
    Resolved = 3,

    /// <summary>
    /// Incident is completely closed and archived
    /// </summary>
    Closed = 4
}