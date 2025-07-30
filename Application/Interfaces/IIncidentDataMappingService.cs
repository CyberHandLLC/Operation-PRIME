using OperationPrime.Application.DTOs;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for incident data mapping operations.
/// Follows Microsoft's 2024 MVVM Community Toolkit service extraction patterns.
/// </summary>
public interface IIncidentDataMappingService
{
    /// <summary>
    /// Maps form data to an Incident entity for creation
    /// </summary>
    /// <param name="formData">The form data from the ViewModel</param>
    /// <returns>A new Incident entity ready for persistence</returns>
    Incident MapToIncident(IncidentFormData formData);

    /// <summary>
    /// Maps an existing Incident entity to form data for editing
    /// </summary>
    /// <param name="incident">The existing incident entity</param>
    /// <returns>Form data populated from the incident</returns>
    IncidentFormData MapFromIncident(Incident incident);

    /// <summary>
    /// Creates a new IncidentFormData with default values
    /// </summary>
    /// <returns>Form data with appropriate defaults</returns>
    IncidentFormData CreateDefaultFormData();

    /// <summary>
    /// Resets form data to default values
    /// </summary>
    /// <param name="formData">The form data to reset</param>
    void ResetFormData(IncidentFormData formData);
}