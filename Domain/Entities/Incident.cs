using System.ComponentModel.DataAnnotations;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Simple incident entity for the incident management system.
/// </summary>
public class Incident
{
    /// <summary>
    /// Gets or sets the unique identifier for the incident.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the incident title.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the incident.
    /// </summary>
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the incident.
    /// </summary>
    public IncidentStatus Status { get; set; } = IncidentStatus.New;

    /// <summary>
    /// Gets or sets the priority level of the incident.
    /// </summary>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// Gets or sets when the incident was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when the incident was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets who created the incident.
    /// </summary>
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who last updated the incident.
    /// </summary>
    [MaxLength(100)]
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Updates the audit fields when the incident is modified.
    /// </summary>
    /// <param name="updatedBy">Who updated the incident.</param>
    public void UpdateAuditFields(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
