using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Provides priority calculation and validation logic.
/// </summary>
public interface IPriorityService
{
    string CalculatePriority(UrgencyLevel urgency, ImpactLevel impact, string? overridePriority = null);
    bool ValidateUrgency(UrgencyLevel urgency);
    bool ValidateImpact(ImpactLevel impact);
}
