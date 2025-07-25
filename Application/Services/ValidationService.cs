using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace OperationPrime.Application.Services;

/// <summary>
/// Provides basic validation for incidents using domain rules.
/// </summary>
public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    public bool Validate(Incident incident)
    {
        var isValid = incident.Validate();
        if (!isValid)
        {
            _logger.LogWarning("Incident {Id} failed validation", incident.Id);
        }
        return isValid;
    }
}
