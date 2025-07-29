using System.ComponentModel.DataAnnotations;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Represents an application that can be affected by incidents.
/// Used for auto-suggestion in the Applications Affected field.
/// </summary>
public class ApplicationInfo
{
    /// <summary>
    /// Unique identifier for the application.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the application.
    /// </summary>
    [Required(ErrorMessage = "Application name is required.")]
    [StringLength(200, ErrorMessage = "Application name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the application.
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether this application is currently active/available.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time when the application record was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Date and time when the application record was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedDate { get; set; }
}
