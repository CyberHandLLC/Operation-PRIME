using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Provides access to Neurons API for incident data.
/// </summary>
public interface INeuronsService
{
    Task<Incident?> FetchIncidentAsync(string incidentNumber);
}
