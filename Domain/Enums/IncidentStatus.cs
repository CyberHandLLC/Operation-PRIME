namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the current status of an incident in its lifecycle.
/// </summary>
public enum IncidentStatus
{
    /// <summary>
    /// Incident has been created but not yet started.
    /// </summary>
    New = 0,

    /// <summary>
    /// Incident is actively being worked on.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Incident has been resolved but not yet closed.
    /// </summary>
    Resolved = 2,

    /// <summary>
    /// Incident has been closed and archived.
    /// </summary>
    Closed = 3
}
