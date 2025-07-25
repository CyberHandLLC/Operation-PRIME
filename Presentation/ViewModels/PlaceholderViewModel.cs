using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.Constants;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Placeholder view shown for unimplemented features.
/// </summary>
public partial class PlaceholderViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Initializes a new instance of the PlaceholderViewModel class.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    public PlaceholderViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        
        // Set default values
        FeatureName = "Feature";
        Message = "This feature is coming soon! We're working hard to bring you the best experience.";
    }

    /// <summary>
    /// Gets or sets the name of the feature that's coming soon.
    /// </summary>
    [ObservableProperty]
    public partial string FeatureName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message to display to the user.
    /// </summary>
    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional details about the feature.
    /// </summary>
    [ObservableProperty]
    public partial string? AdditionalDetails { get; set; }

    /// <summary>
    /// Gets or sets the estimated completion timeframe.
    /// </summary>
    [ObservableProperty]
    public partial string? EstimatedCompletion { get; set; }

    /// <summary>
    /// Command to navigate back to the dashboard.
    /// </summary>
    [RelayCommand]
    private void BackToDashboard()
    {
        _navigationService.NavigateTo(NavigationConstants.Dashboard);
    }

    /// <summary>
    /// Command to go back to the previous view.
    /// </summary>
    [RelayCommand]
    private void GoBack()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
        else
        {
            // Fallback to dashboard if no history
            BackToDashboard();
        }
    }

    /// <summary>
    /// Sets the feature information for this placeholder.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="additionalDetails">Additional details about the feature.</param>
    /// <param name="estimatedCompletion">Estimated completion timeframe.</param>
    public void SetFeatureInfo(string featureName, string? message = null, string? additionalDetails = null, string? estimatedCompletion = null)
    {
        FeatureName = featureName;
        
        if (!string.IsNullOrEmpty(message))
            Message = message;
        
        AdditionalDetails = additionalDetails;
        EstimatedCompletion = estimatedCompletion;
    }
}
