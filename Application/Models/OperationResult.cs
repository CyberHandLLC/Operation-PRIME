namespace OperationPrime.Application.Models;

/// <summary>
/// Result wrapper for operations that can succeed or fail.
/// Follows the Result pattern for consistent error handling across the Application layer.
/// </summary>
/// <typeparam name="T">The type of the result value</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the result value if the operation was successful.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the collection of validation errors if validation failed.
    /// </summary>
    public List<string> ValidationErrors { get; init; } = new();

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <param name="value">The result value</param>
    /// <returns>A successful operation result</returns>
    public static OperationResult<T> Success(T value) => new() { IsSuccess = true, Value = value };

    /// <summary>
    /// Creates a failed operation result.
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A failed operation result</returns>
    public static OperationResult<T> Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };

    /// <summary>
    /// Creates a validation failure operation result.
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <returns>A validation failure operation result</returns>
    public static OperationResult<T> ValidationFailure(List<string> validationErrors) => new() 
    { 
        IsSuccess = false, 
        ValidationErrors = validationErrors 
    };

    /// <summary>
    /// Creates a validation failure operation result with a single error.
    /// </summary>
    /// <param name="validationError">The validation error</param>
    /// <returns>A validation failure operation result</returns>
    public static OperationResult<T> ValidationFailure(string validationError) => new() 
    { 
        IsSuccess = false, 
        ValidationErrors = [validationError] 
    };
}