namespace OperationPrime.Domain.Enums;

/// <summary>
/// Defines impact levels for priority matrix calculation.
/// Used in combination with Urgency to determine overall Priority.
/// </summary>
public enum ImpactLevel
{
    /// <summary>
    /// Low impact - affects few users or non-critical systems
    /// </summary>
    Low = 3,

    /// <summary>
    /// Medium impact - affects moderate number of users or important systems
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High impact - affects many users or critical business systems
    /// </summary>
    High = 1
}