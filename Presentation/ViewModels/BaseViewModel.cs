using CommunityToolkit.Mvvm.ComponentModel;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// Base class for all ViewModels providing common MVVM functionality.
/// Inherits from ObservableValidator for property change notifications and validation.
/// </summary>
public class BaseViewModel : ObservableValidator
{
    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is currently busy/loading.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Gets or sets the title for the current view.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Called when the view is loaded.
    /// Override in derived classes to perform initialization.
    /// </summary>
    public virtual Task OnViewLoadedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the view is unloaded.
    /// Override in derived classes to perform cleanup.
    /// </summary>
    public virtual Task OnViewUnloadedAsync()
    {
        return Task.CompletedTask;
    }
}
