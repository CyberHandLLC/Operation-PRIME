namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the type of incident in the unified incident entity approach.
/// Pre-incidents can be promoted to major incidents through the conversion workflow.
/// </summary>
public enum IncidentType
{
    /// <summary>
    /// Pre-incident: Potential issue identified before it becomes a major incident.
    /// Used for proactive incident management and prevention.
    /// </summary>
    PreIncident = 1,

    /// <summary>
    /// Major incident: Active incident requiring immediate response and resolution.
    /// Can be created directly or promoted from a pre-incident.
    /// </summary>
    MajorIncident = 2
}
