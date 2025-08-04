using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Presentation.Constants;
using OperationPrime.Presentation.ViewModels.Base;
using OperationPrime.Presentation.Extensions;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the incident list view.
/// Handles data loading and user interactions for incident management.
/// Follows MVVM Community Toolkit patterns with async data loading.
/// Inherits from BaseValidatingViewModel for consistent error handling and validation patterns.
/// </summary>
public partial class IncidentListViewModel : BaseValidatingViewModel
{
    private readonly IIncidentService _incidentService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<IncidentListViewModel> _logger;

    /// <summary>
    /// Collection of incidents to display in the UI.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<Incident> Incidents { get; set; } = new();

    /// <summary>
    /// Indicates whether data is currently being loaded.
    /// Hides the inherited IsLoading property from BaseValidatingViewModel to maintain existing binding compatibility.
    /// </summary>
    [ObservableProperty]
    public new partial bool IsLoading { get; set; }

    // Note: ErrorMessage and SuccessMessage are inherited from BaseValidatingViewModel

    /// <summary>
    /// Initializes a new instance of the IncidentListViewModel.
    /// </summary>
    /// <param name="incidentService">Service for incident data operations.</param>
    /// <param name="navigationService">Service for navigation operations.</param>
    /// <param name="logger">Logger for this ViewModel.</param>
    public IncidentListViewModel(
        IIncidentService incidentService, 
        INavigationService navigationService,
        ILogger<IncidentListViewModel> logger)
    {
        _incidentService = incidentService;
        _navigationService = navigationService;
        _logger = logger;
        
        _logger.LogDebug("Initializing IncidentListViewModel");
    }

    /// <summary>
    /// Loads incidents from the service asynchronously with cancellation support.
    /// Follows .NET 9/WinUI 3 async patterns with enhanced user experience.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true)]
    private async Task LoadIncidentsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting incident list loading process");
        
        try
        {
            IsLoading = true;
            ClearMessages();

            var incidentData = await _incidentService.GetAllAsync(cancellationToken).ConfigureAwait(true);
            
            // Check for cancellation before UI updates
            cancellationToken.ThrowIfCancellationRequested();
            
            // Use extension method for better performance
            Incidents.LoadFrom(incidentData);
            
            _logger.LogInformation("Successfully loaded {IncidentCount} incidents", Incidents.Count);
            SuccessMessage = $"Loaded {Incidents.Count} incidents successfully";
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully - don't show error for user-initiated cancellation
            _logger.LogInformation("Incident loading was cancelled by user");
            ClearMessages();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load incidents");
            SetErrorMessage($"Failed to load incidents: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            _logger.LogDebug("Incident loading process completed, IsLoading set to false");
        }
    }

    /// <summary>
    /// Refreshes the incident data by reloading from the service with cancellation support.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true)]
    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("User requested incident list refresh");
        await LoadIncidentsAsync(cancellationToken).ConfigureAwait(true);
    }

    /// <summary>
    /// Navigates to the incident creation page.
    /// </summary>
    [RelayCommand]
    private void CreateIncident()
    {
        _logger.LogDebug("Navigating to incident creation page");
        _navigationService.NavigateTo(NavigationKeys.IncidentCreate);
    }
}
