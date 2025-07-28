using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Unified incident entity supporting both pre-incidents and major incidents.
/// Follows the architectural decision for a single entity with type differentiation.
/// </summary>
public class Incident
{
    /// <summary>
    /// Unique identifier for the incident.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Brief title describing the incident.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of incident (Pre-Incident or Major Incident).
    /// </summary>
    public IncidentType IncidentType { get; set; }

    /// <summary>
    /// Priority level of the incident (P1-P4).
    /// </summary>
    public Priority Priority { get; set; }

    /// <summary>
    /// Current status of the incident.
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// Date and time when the incident was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
