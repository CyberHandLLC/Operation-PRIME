using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Services;

/// <summary>
/// Calculates incident priority using the urgency x impact matrix.
/// </summary>
public class PriorityService : IPriorityService
{
    private readonly ILogger<PriorityService> _logger;

    private readonly string[,] _matrix = new string[3, 3]
    {
        { "P1", "P1", "P2" },
        { "P1", "P2", "P3" },
        { "P2", "P3", "P4" }
    };

    public PriorityService(ILogger<PriorityService> logger)
    {
        _logger = logger;
    }

    public string CalculatePriority(UrgencyLevel urgency, ImpactLevel impact, string? overridePriority = null)
    {
        if (!ValidateUrgency(urgency) || !ValidateImpact(impact))
        {
            _logger.LogWarning("Invalid inputs for priority calculation: {Urgency} {Impact}", urgency, impact);
            return "P4";
        }

        var calculated = _matrix[(int)urgency, (int)impact];
        var finalPriority = string.IsNullOrWhiteSpace(overridePriority) ? calculated : overridePriority!;
        _logger.LogDebug("Priority calculated as {Priority}", finalPriority);
        return finalPriority;
    }

    public bool ValidateUrgency(UrgencyLevel urgency) => Enum.IsDefined(typeof(UrgencyLevel), urgency);

    public bool ValidateImpact(ImpactLevel impact) => Enum.IsDefined(typeof(ImpactLevel), impact);
}
