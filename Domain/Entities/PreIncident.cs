using System.ComponentModel.DataAnnotations;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Represents a pre-incident - a potential issue that may escalate to a major incident.
/// </summary>
public class PreIncident : Incident
{
    /// <summary>
    /// Gets the type of incident (always PreIncident for this class).
    /// </summary>
    public override IncidentType Type => IncidentType.PreIncident;

    /// <summary>
    /// Gets or sets the name of the person who identified the issue.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string IdentifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the potential impact description if this escalates.
    /// </summary>
    [MaxLength(1000)]
    public string? PotentialImpact { get; set; }

    /// <summary>
    /// Gets or sets whether this pre-incident has been escalated to a major incident.
    /// </summary>
    public bool IsEscalated { get; set; }

    /// <summary>
    /// Gets or sets the ID of the major incident if escalated.
    /// </summary>
    public Guid? EscalatedToIncidentId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when escalated to major incident.
    /// </summary>
    public DateTime? EscalatedDateTime { get; set; }

    /// <summary>
    /// Gets or sets the preventive actions taken or recommended.
    /// </summary>
    [MaxLength(1000)]
    public string? PreventiveActions { get; set; }

    /// <summary>
    /// Gets or sets the risk assessment notes.
    /// </summary>
    [MaxLength(1000)]
    public string? RiskAssessment { get; set; }

    /// <summary>
    /// Validates the pre-incident data.
    /// </summary>
    /// <returns>True if valid, false otherwise.</returns>
    public override bool Validate()
    {
        if (!base.Validate())
            return false;

        if (string.IsNullOrWhiteSpace(IdentifiedBy))
            return false;

        // If escalated, must have escalation details
        if (IsEscalated && (!EscalatedToIncidentId.HasValue || !EscalatedDateTime.HasValue))
            return false;

        return true;
    }

    /// <summary>
    /// Escalates this pre-incident to a major incident.
    /// </summary>
    /// <param name="majorIncidentId">The ID of the created major incident.</param>
    /// <param name="escalatedBy">The user who escalated the incident.</param>
    public void EscalateToMajorIncident(Guid majorIncidentId, string escalatedBy)
    {
        IsEscalated = true;
        EscalatedToIncidentId = majorIncidentId;
        EscalatedDateTime = DateTime.UtcNow;
        UpdateAuditFields(escalatedBy);
    }
}
