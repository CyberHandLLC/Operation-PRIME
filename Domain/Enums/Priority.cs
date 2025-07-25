namespace OperationPrime.Domain.Enums;

/// <summary>
/// Defines incident priority levels based on impact and urgency matrix.
/// Priority 1 = High Impact + High Urgency (Critical)
/// Priority 2 = High Impact + Medium Urgency OR Medium Impact + High Urgency
/// Priority 3 = Low priority incidents
/// </summary>
public enum Priority
{
    /// <summary>
    /// Critical priority - immediate attention required
    /// High Impact + High Urgency
    /// </summary>
    High = 1,

    /// <summary>
    /// Medium priority - should be addressed promptly
    /// High Impact + Medium Urgency OR Medium Impact + High Urgency
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Low priority - can be scheduled for later resolution
    /// Low Impact or Low Urgency combinations
    /// </summary>
    Low = 3
}