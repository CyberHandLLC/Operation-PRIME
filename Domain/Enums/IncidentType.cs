namespace OperationPrime.Domain.Enums;

/// <summary>
/// Defines the type of incident being reported.
/// Used to determine workflow paths and required fields.
/// </summary>
public enum IncidentType
{
    /// <summary>
    /// Minor incident with limited impact (≤100 users)
    /// </summary>
    PreIncident = 1,

    /// <summary>
    /// Major incident with significant impact (>100 users or critical systems)
    /// </summary>
    MajorIncident = 2
}