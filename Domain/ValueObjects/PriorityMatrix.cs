using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.ValueObjects;

/// <summary>
/// Value object that encapsulates priority calculation logic based on Impact and Urgency matrix.
/// Implements the business rule: Priority = f(Impact, Urgency)
/// </summary>
public readonly record struct PriorityMatrix
{
    public ImpactLevel Impact { get; }
    public UrgencyLevel Urgency { get; }
    public Priority CalculatedPriority { get; }

    public PriorityMatrix(ImpactLevel impact, UrgencyLevel urgency)
    {
        Impact = impact;
        Urgency = urgency;
        CalculatedPriority = CalculatePriority(impact, urgency);
    }

    /// <summary>
    /// Calculates priority based on Impact and Urgency matrix:
    /// High Impact + High Urgency = High Priority (1)
    /// High Impact + Medium Urgency OR Medium Impact + High Urgency = Medium Priority (2)
    /// All other combinations = Low Priority (3)
    /// </summary>
    private static Priority CalculatePriority(ImpactLevel impact, UrgencyLevel urgency)
    {
        return (impact, urgency) switch
        {
            (ImpactLevel.High, UrgencyLevel.High) => Priority.High,
            (ImpactLevel.High, UrgencyLevel.Medium) => Priority.Medium,
            (ImpactLevel.Medium, UrgencyLevel.High) => Priority.Medium,
            _ => Priority.Low
        };
    }

    public override string ToString() => $"Priority {(int)CalculatedPriority} (Impact: {Impact}, Urgency: {Urgency})";
}