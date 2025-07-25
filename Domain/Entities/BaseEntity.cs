using System.ComponentModel.DataAnnotations;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Base entity class providing common audit trail properties for all domain entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the username or identifier of who created the entity.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// Gets or sets the username or identifier of who last updated the entity.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UpdatedBy { get; set; } = "System";

    /// <summary>
    /// Updates the audit fields for entity modification.
    /// </summary>
    /// <param name="updatedBy">The username or identifier of who is updating the entity.</param>
    public virtual void UpdateAuditFields(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
