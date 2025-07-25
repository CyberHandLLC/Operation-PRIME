namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the type of incident being tracked.
/// Determines which workflow and fields are applicable.
/// </summary>
public enum IncidentType
{
    /// <summary>
    /// Pre-incident - potential issue that may become a major incident.
    /// </summary>
    PreIncident = 0,

    /// <summary>
    /// Major incident - significant disruption requiring immediate response.
    /// </summary>
    MajorIncident = 1
}
