namespace OperationPrime.Domain.Enums;

/// <summary>
/// Source from which the incident was reported.
/// Used to drive workflow routing and additional field visibility.
/// </summary>
public enum IncidentSource
{
    /// <summary>
    /// Service Desk reported incident.
    /// </summary>
    ServiceDesk = 1,

    /// <summary>
    /// Network Operations Center reported incident.
    /// </summary>
    NOC = 2,

    /// <summary>
    /// Subject Matter Expert reported incident.
    /// </summary>
    SME = 3,

    /// <summary>
    /// Business escalation source.
    /// </summary>
    BusinessEscalation = 4,

    /// <summary>
    /// Other/unspecified source.
    /// </summary>
    Other = 5
}



