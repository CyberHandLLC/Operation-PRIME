using CommunityToolkit.Mvvm.ComponentModel;

namespace OperationPrime.Presentation.ViewModels.Base;

/// <summary>
/// Base ViewModel class providing common validation patterns and error handling.
/// Follows Microsoft MVVM Community Toolkit best practices.
/// </summary>
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
