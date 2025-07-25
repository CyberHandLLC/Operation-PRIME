using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.ValueObjects;

/// <summary>
/// Value object representing the priority matrix calculation logic.
/// Determines priority based on urgency and impact levels.
/// </summary>
public readonly struct PriorityMatrix
{
    /// <summary>
    /// Gets the urgency level.
    /// </summary>
    public UrgencyLevel Urgency { get; }

    /// <summary>
    /// Gets the impact level.
    /// </summary>
    public ImpactLevel Impact { get; }

    /// <summary>
    /// Gets the calculated priority based on urgency and impact.
    /// </summary>
    public Priority CalculatedPriority { get; }

    /// <summary>
    /// Initializes a new instance of the PriorityMatrix struct.
    /// </summary>
    /// <param name="urgency">The urgency level.</param>
    /// <param name="impact">The impact level.</param>
    public PriorityMatrix(UrgencyLevel urgency, ImpactLevel impact)
    {
        Urgency = urgency;
        Impact = impact;
        CalculatedPriority = CalculatePriority(urgency, impact);
    }

    /// <summary>
    /// Calculates the priority based on urgency and impact using the standard matrix.
    /// </summary>
    /// <param name="urgency">The urgency level.</param>
    /// <param name="impact">The impact level.</param>
    /// <returns>The calculated priority.</returns>
    private static Priority CalculatePriority(UrgencyLevel urgency, ImpactLevel impact)
    {
        // Priority Matrix:
        // High/High = High Priority
        // High/Medium, Medium/High = High Priority  
        // Medium/Medium = Medium Priority
        // Low/High, High/Low = Medium Priority
        // Low/Medium, Medium/Low = Low Priority
        // Low/Low = Low Priority

        return (urgency, impact) switch
        {
            (UrgencyLevel.High, ImpactLevel.High) => Priority.High,
            (UrgencyLevel.High, ImpactLevel.Medium) => Priority.High,
            (UrgencyLevel.Medium, ImpactLevel.High) => Priority.High,
            (UrgencyLevel.Medium, ImpactLevel.Medium) => Priority.Medium,
            (UrgencyLevel.Low, ImpactLevel.High) => Priority.Medium,
            (UrgencyLevel.High, ImpactLevel.Low) => Priority.Medium,
            (UrgencyLevel.Low, ImpactLevel.Medium) => Priority.Low,
            (UrgencyLevel.Medium, ImpactLevel.Low) => Priority.Low,
            (UrgencyLevel.Low, ImpactLevel.Low) => Priority.Low,
            _ => Priority.Medium // Default fallback
        };
    }

    /// <summary>
    /// Creates a new PriorityMatrix instance.
    /// </summary>
    /// <param name="urgency">The urgency level.</param>
    /// <param name="impact">The impact level.</param>
    /// <returns>A new PriorityMatrix instance.</returns>
    public static PriorityMatrix Create(UrgencyLevel urgency, ImpactLevel impact)
    {
        return new PriorityMatrix(urgency, impact);
    }

    /// <summary>
    /// Gets a description of the priority calculation rationale.
    /// </summary>
    /// <returns>A string describing why this priority was assigned.</returns>
    public string GetCalculationRationale()
    {
        return $"Priority {CalculatedPriority} assigned based on {Urgency} urgency and {Impact} impact";
    }

    /// <summary>
    /// Determines whether two PriorityMatrix instances are equal.
    /// </summary>
    /// <param name="other">The other PriorityMatrix to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public bool Equals(PriorityMatrix other)
    {
        return Urgency == other.Urgency && Impact == other.Impact;
    }

    /// <summary>
    /// Determines whether this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public override bool Equals(object? obj)
    {
        return obj is PriorityMatrix other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for this instance.
    /// </summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Urgency, Impact);
    }

    /// <summary>
    /// Returns a string representation of the priority matrix.
    /// </summary>
    /// <returns>A string representation.</returns>
    public override string ToString()
    {
        return $"PriorityMatrix: {Urgency} Urgency + {Impact} Impact = {CalculatedPriority} Priority";
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(PriorityMatrix left, PriorityMatrix right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(PriorityMatrix left, PriorityMatrix right)
    {
        return !left.Equals(right);
    }
}
