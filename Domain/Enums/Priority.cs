namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the priority level of an incident.
/// Used in priority matrix calculations along with urgency and impact.
/// Follows P1 (highest) to P4 (lowest) priority scale.
/// </summary>
public enum Priority
{
    /// <summary>
    /// P1 - Critical priority, requires immediate attention.
    /// Typically for security incidents, data center outages, or high-impact issues.
    /// </summary>
    P1 = 1,

    /// <summary>
    /// P2 - High priority, should be addressed quickly.
    /// Significant business impact but not critical.
    /// </summary>
    P2 = 2,

    /// <summary>
    /// P3 - Medium priority, normal response time.
    /// Moderate business impact, can be scheduled.
    /// </summary>
    P3 = 3,

    /// <summary>
    /// P4 - Low priority, can be addressed during normal business hours.
    /// Minimal business impact, low urgency.
    /// </summary>
    P4 = 4
}
