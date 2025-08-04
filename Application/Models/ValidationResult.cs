namespace OperationPrime.Application.Models;

/// <summary>
/// Result wrapper for validation operations.
/// Provides detailed validation error information for business rule validation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the collection of validation error messages.
    /// </summary>
    public List<string> Errors { get; init; } = new();

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A successful validation result</returns>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result with multiple errors.
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>A failed validation result</returns>
    public static ValidationResult Failure(List<string> errors) => new() { IsValid = false, Errors = errors };

    /// <summary>
    /// Creates a failed validation result with a single error.
    /// </summary>
    /// <param name="error">The validation error</param>
    /// <returns>A failed validation result</returns>
    public static ValidationResult Failure(string error) => new() { IsValid = false, Errors = [error] };
}