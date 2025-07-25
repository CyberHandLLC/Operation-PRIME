namespace OperationPrime.Domain.Entities;

/// <summary>
/// Base entity class providing common properties for all domain entities.
/// Includes audit trail and identity management.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// When the entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// When the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Who created the entity (for audit trail)
    /// </summary>
    public string CreatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// Who last updated the entity (for audit trail)
    /// </summary>
    public string UpdatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// Updates the audit trail when entity is modified
    /// </summary>
    protected void UpdateAuditTrail(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Sets the creator information (typically called once during entity creation)
    /// </summary>
    protected void SetCreator(string createdBy)
    {
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }
}