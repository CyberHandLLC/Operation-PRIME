using System.ComponentModel.DataAnnotations;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Represents an entry in the audit trail for an incident.
/// </summary>
public class AuditLogEntry
{
    /// <summary>
    /// Gets or sets the unique audit identifier.
    /// </summary>
    [Key]
    public Guid AuditId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the associated incident identifier.
    /// </summary>
    [Required]
    public Guid IncidentId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the change.
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the user who made the change.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field that changed.
    /// </summary>
    [MaxLength(100)]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the old value, if any.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value, if any.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the action type (e.g., Created, Updated, Deleted).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;
}
