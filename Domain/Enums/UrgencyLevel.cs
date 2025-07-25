namespace OperationPrime.Domain.Enums;

/// <summary>
/// Defines urgency levels for priority matrix calculation.
/// Used in combination with Impact to determine overall Priority.
/// </summary>
public enum UrgencyLevel
{
    /// <summary>
    /// Low urgency - can wait for scheduled resolution
    /// </summary>
    Low = 3,

    /// <summary>
    /// Medium urgency - should be addressed in reasonable timeframe
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High urgency - requires immediate attention
    /// </summary>
    High = 1
}