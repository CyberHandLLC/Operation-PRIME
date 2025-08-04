using OperationPrime.Application.DTOs;
using OperationPrime.Application.Models;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for orchestrating complete incident operations.
/// Coordinates multiple services to handle complex business workflows.
/// </summary>
public interface IIncidentOrchestrationService
{
    /// <summary>
    /// Creates a new incident with full business logic validation and processing.
    /// </summary>
    /// <param name="formData">The incident form data</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Result containing the created incident or error information</returns>
    Task<OperationResult<Incident>> CreateIncidentAsync(IncidentFormData formData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes default values for a new incident form.
    /// </summary>
    /// <returns>Form data with appropriate defaults</returns>
    IncidentFormData CreateDefaultIncidentForm();
}