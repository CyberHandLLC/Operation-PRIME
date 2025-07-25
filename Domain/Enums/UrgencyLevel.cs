namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the urgency level of an incident.
/// Used in priority matrix calculations to determine overall priority.
/// </summary>
public enum UrgencyLevel
{
    /// <summary>
    /// Low urgency - can wait for resolution.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium urgency - should be resolved within reasonable timeframe.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High urgency - requires immediate resolution.
    /// </summary>
    High = 2
}
