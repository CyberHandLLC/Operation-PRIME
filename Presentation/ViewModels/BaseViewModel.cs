using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// Base class for all ViewModels providing common functionality
/// </summary>
public abstract partial class BaseViewModel : ObservableValidator
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasErrors;

    /// <summary>
    /// Gets a value indicating whether the ViewModel is in a ready state (not busy and not loading)
    /// </summary>
    public bool IsReady => !IsBusy && !IsLoading;

    /// <summary>
    /// Sets the busy state and optionally clears errors
    /// </summary>
    /// <param name="isBusy">Whether the ViewModel is busy</param>
    /// <param name="clearErrors">Whether to clear existing errors</param>
    protected void SetBusyState(bool isBusy, bool clearErrors = true)
    {
        IsBusy = isBusy;
        
        if (clearErrors && isBusy)
        {
            ClearErrors();
        }
        
        OnPropertyChanged(nameof(IsReady));
    }

    /// <summary>
    /// Sets the loading state and optionally clears errors
    /// </summary>
    /// <param name="isLoading">Whether the ViewModel is loading</param>
    /// <param name="clearErrors">Whether to clear existing errors</param>
    protected void SetLoadingState(bool isLoading, bool clearErrors = true)
    {
        IsLoading = isLoading;
        
        if (clearErrors && isLoading)
        {
            ClearErrors();
        }
        
        OnPropertyChanged(nameof(IsReady));
    }

    /// <summary>
    /// Sets an error message and updates error state
    /// </summary>
    /// <param name="errorMessage">The error message to display</param>
    protected void SetError(string? errorMessage)
    {
        ErrorMessage = errorMessage;
        HasErrors = !string.IsNullOrEmpty(errorMessage);
    }

    /// <summary>
    /// Clears all errors
    /// </summary>
    protected void ClearErrors()
    {
        ErrorMessage = null;
        HasErrors = false;
        ClearErrors(string.Empty); // Clear validation errors
    }

    /// <summary>
    /// Executes an async operation with automatic busy state management and error handling
    /// </summary>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="showLoading">Whether to show loading state instead of busy state</param>
    protected async Task ExecuteAsync(Func<Task> operation, bool showLoading = false)
    {
        try
        {
            if (showLoading)
                SetLoadingState(true);
            else
                SetBusyState(true);

            await operation();
        }
        catch (Exception ex)
        {
            SetError($"An error occurred: {ex.Message}");
        }
        finally
        {
            if (showLoading)
                SetLoadingState(false);
            else
                SetBusyState(false);
        }
    }

    /// <summary>
    /// Executes an async operation with return value, automatic busy state management and error handling
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="showLoading">Whether to show loading state instead of busy state</param>
    /// <returns>The result of the operation, or default(T) if an error occurred</returns>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, bool showLoading = false)
    {
        try
        {
            if (showLoading)
                SetLoadingState(true);
            else
                SetBusyState(true);

            return await operation();
        }
        catch (Exception ex)
        {
            SetError($"An error occurred: {ex.Message}");
            return default;
        }
        finally
        {
            if (showLoading)
                SetLoadingState(false);
            else
                SetBusyState(false);
        }
    }

    /// <summary>
    /// Command to clear errors manually
    /// </summary>
    [RelayCommand]
    private void ClearError()
    {
        ClearErrors();
    }
}
