namespace OperationPrime.Domain.Enums;

/// <summary>
/// Represents the impact level of an incident on business operations.
/// Used in priority matrix calculations to determine overall priority.
/// </summary>
public enum ImpactLevel
{
    /// <summary>
    /// Low impact - affects individual users or non-critical systems.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium impact - affects department or multiple users.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High impact - affects entire organization or critical systems.
    /// </summary>
    High = 2
}
