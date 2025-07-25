namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the priority level of an incident.
/// Used in priority matrix calculations along with urgency and impact.
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority - can be addressed during normal business hours.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium priority - should be addressed soon but not immediately.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High priority - requires immediate attention and resources.
    /// </summary>
    High = 2
}
