using OperationPrime.Application.DTOs;
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
    /// Validates complete incident form data across all business rules.
    /// </summary>
    /// <param name="formData">The incident form data to validate</param>
    /// <returns>Validation result with any error messages</returns>
    ValidationResult ValidateIncidentData(IncidentFormData formData);

    /// <summary>
    /// Initializes default values for a new incident form.
    /// </summary>
    /// <returns>Form data with appropriate defaults</returns>
    IncidentFormData CreateDefaultIncidentForm();
}

/// <summary>
/// Result wrapper for operations that can succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the result value</typeparam>
public class OperationResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> ValidationErrors { get; init; } = new();

    public static OperationResult<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static OperationResult<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static OperationResult<T> ValidationFailure(List<string> errors) => new() { IsSuccess = false, ValidationErrors = errors };
}

/// <summary>
/// Result for validation operations.
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; init; }
    public List<string> Errors { get; init; } = new();
    public string? FirstError => Errors.FirstOrDefault();

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(List<string> errors) => new() { IsValid = false, Errors = errors };
    public static ValidationResult Failure(string error) => new() { IsValid = false, Errors = [error] };
} 