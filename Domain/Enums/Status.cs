namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the current status of an incident.
/// Simple status progression for incident lifecycle management.
/// </summary>
public enum Status
{
    /// <summary>
    /// New incident that has been created but not yet assigned or started.
    /// </summary>
    New = 1,

    /// <summary>
    /// Incident is currently being worked on and is in progress.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Incident has been resolved and solution has been implemented.
    /// </summary>
    Resolved = 3,

    /// <summary>
    /// Incident has been closed and no further action is required.
    /// </summary>
    Closed = 4
}
