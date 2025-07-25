using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Provides validation services for incidents.
/// </summary>
public interface IValidationService
{
    bool Validate(Incident incident);
}
