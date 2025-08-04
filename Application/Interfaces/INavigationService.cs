using Microsoft.UI.Xaml.Controls;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Navigation service interface for managing application navigation
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets the current navigation frame
    /// </summary>
    Frame? Frame { get; }

    /// <summary>
    /// Sets the navigation frame
    /// </summary>
    /// <param name="frame">The frame to use for navigation</param>
    void SetFrame(Frame frame);

    /// <summary>
    /// Navigates to the specified page type
    /// </summary>
    /// <param name="pageType">The type of page to navigate to</param>
    /// <param name="parameter">Optional parameter to pass to the page</param>
    /// <returns>True if navigation was successful</returns>
    bool NavigateTo(Type pageType, object? parameter = null);

    /// <summary>
    /// Navigates to the specified page type by name
    /// </summary>
    /// <param name="pageKey">The key/name of the page to navigate to</param>
    /// <param name="parameter">Optional parameter to pass to the page</param>
    /// <returns>True if navigation was successful</returns>
    bool NavigateTo(string pageKey, object? parameter = null);

    /// <summary>
    /// Navigates back if possible
    /// </summary>
    /// <returns>True if navigation back was successful</returns>
    bool GoBack();

    /// <summary>
    /// Gets whether the frame can navigate back
    /// </summary>
    bool CanGoBack { get; }



    /// <summary>
    /// Event raised when navigation occurs
    /// </summary>
    event EventHandler<string>? NavigationChanged;
}
