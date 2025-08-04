using CommunityToolkit.Mvvm.ComponentModel;

namespace OperationPrime.Presentation.ViewModels.Base;

/// <summary>
/// Interface for ViewModels that require async initialization.
/// Follows 2024 best practices for async ViewModel initialization.
/// </summary>
/// <remarks>
/// Implement this interface when your ViewModel needs to perform async operations
/// during initialization, such as loading data from services or external APIs.
/// The pattern separates construction (fast, synchronous) from initialization (potentially slow, asynchronous).
/// </remarks>
/// <example>
/// <code>
/// public async Task InitializeAsync()
/// {
///     if (IsInitialized) return;
///     
///     try
///     {
///         await LoadDataAsync();
///         IsInitialized = true;
///     }
///     catch (Exception ex)
///     {
///         ErrorMessage = "Failed to initialize";
///         IsInitialized = false;
///     }
/// }
/// </code>
/// </example>
public interface IAsyncInitializable
{
    /// <summary>
    /// Initializes the ViewModel asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeAsync();
    
    /// <summary>
    /// Gets a value indicating whether the ViewModel has been successfully initialized.
    /// </summary>
    bool IsInitialized { get; }
}

/// <summary>
/// Base ViewModel class providing common validation patterns and error handling.
/// Follows Microsoft MVVM Community Toolkit best practices.
/// </summary>
/// <remarks>
/// <para>
/// This base class provides a consistent foundation for ViewModels that need validation,
/// error handling, and user feedback capabilities. It inherits from ObservableValidator
/// to provide automatic validation support through data annotations.
/// </para>
/// <para>
/// Key features:
/// - Automatic property change notifications via [ObservableProperty]
/// - Built-in validation through ObservableValidator
/// - Standardized error and success message handling
/// - Loading state management
/// - Helper methods for common validation scenarios
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public partial class MyViewModel : BaseValidatingViewModel
/// {
///     [ObservableProperty]
///     [Required(ErrorMessage = "Name is required")]
///     public partial string Name { get; set; } = string.Empty;
///     
///     [RelayCommand]
///     private async Task SaveAsync()
///     {
///         try
///         {
///             IsLoading = true;
///             ClearMessages();
///             
///             if (!ValidateAllProperties())
///             {
///                 SetErrorMessage("Please fix validation errors");
///                 return;
///             }
///             
///             await SaveToServiceAsync();
///             SetSuccessMessage("Saved successfully!");
///         }
///         catch (Exception ex)
///         {
///             SetErrorMessage($"Save failed: {ex.Message}");
///         }
///         finally
///         {
///             IsLoading = false;
///         }
///     }
/// }
/// </code>
/// </example>
public abstract partial class BaseValidatingViewModel : ObservableValidator
{
    /// <summary>
    /// Indicates whether the ViewModel is currently performing an operation.
    /// Can be used to show loading indicators and disable UI elements.
    /// </summary>
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    /// <summary>
    /// Error message to display to the user when operations fail.
    /// </summary>
    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    /// <summary>
    /// Success message to display to the user when operations complete successfully.
    /// </summary>
    [ObservableProperty]
    public partial string? SuccessMessage { get; set; }

    /// <summary>
    /// Helper method to validate multiple properties and clear their errors.
    /// This provides a convenient way to validate and clear errors for multiple properties at once.
    /// </summary>
    /// <param name="propertyNames">Names of properties to validate.</param>
    /// <remarks>
    /// Uses reflection to get property values. For performance-critical scenarios with many properties,
    /// consider calling ValidateProperty directly for each property.
    /// </remarks>
    protected void ValidateProperties(params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            ClearErrors(propertyName);
            var value = GetType().GetProperty(propertyName)?.GetValue(this);
            ValidateProperty(value, propertyName);
        }
    }

    /// <summary>
    /// Validates all properties that have validation attributes.
    /// Clears existing errors before validation.
    /// </summary>
    /// <remarks>
    /// This method wraps the base ObservableValidator.ValidateAllProperties() method
    /// which automatically discovers and validates all properties with validation attributes.
    /// Uses the 'new' modifier to explicitly hide the base implementation.
    /// </remarks>
    protected new void ValidateAllProperties()
    {
        base.ValidateAllProperties();
    }

    /// <summary>
    /// Clears all error and success messages.
    /// Useful when starting new operations or resetting form state.
    /// </summary>
    protected void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }

    /// <summary>
    /// Sets an error message and clears any success message.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    protected void SetErrorMessage(string message)
    {
        ErrorMessage = message;
        SuccessMessage = null;
    }

    /// <summary>
    /// Sets a success message and clears any error message.
    /// </summary>
    /// <param name="message">The success message to display.</param>
    protected void SetSuccessMessage(string message)
    {
        SuccessMessage = message;
        ErrorMessage = null;
    }
}
