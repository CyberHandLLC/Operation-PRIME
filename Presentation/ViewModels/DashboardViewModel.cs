using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.Constants;
using OperationPrime.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Dashboard view providing metrics and quick actions.
/// </summary>
public partial class DashboardViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<DashboardViewModel> _logger;

    /// <summary>
    /// Initializes a new instance of the DashboardViewModel class.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    public DashboardViewModel(INavigationService navigationService, ILogger<DashboardViewModel> logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger;
        
        // Initialize with sample data - will be replaced with real data later
        InitializeSampleData();
        _logger.LogDebug("DashboardViewModel initialized");
    }

    /// <summary>
    /// Gets or sets the total number of active incidents.
    /// </summary>
    [ObservableProperty]
    public partial int ActiveIncidents { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of high priority incidents.
    /// </summary>
    [ObservableProperty]
    public partial int HighPriorityIncidents { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of incidents resolved today.
    /// </summary>
    [ObservableProperty]
    public partial int ResolvedToday { get; set; } = 0;

    /// <summary>
    /// Gets or sets the average resolution time in hours.
    /// </summary>
    [ObservableProperty]
    public partial double AverageResolutionTime { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets a value indicating whether data is currently loading.
    /// </summary>
    [ObservableProperty]
    public partial bool IsLoading { get; set; } = false;

    /// <summary>
    /// Gets or sets the last refresh time.
    /// </summary>
    [ObservableProperty]
    public partial DateTime LastRefreshTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Command to launch the pre-incident wizard.
    /// </summary>
    [RelayCommand]
    private void CreatePreIncident()
    {
        _logger.LogDebug("Navigating to IncidentWizard view for Pre-Incident");
        _navigationService.NavigateTo(NavigationConstants.CreateIncident, IncidentType.PreIncident);
    }

    /// <summary>
    /// Command to launch the major incident wizard.
    /// </summary>
    [RelayCommand]
    private void CreateMajorIncident()
    {
        _logger.LogDebug("Navigating to IncidentWizard view for Major Incident");
        _navigationService.NavigateTo(NavigationConstants.CreateIncident, IncidentType.MajorIncident);
    }

    /// <summary>
    /// Command to view all incidents.
    /// </summary>
    [RelayCommand]
    private void ViewIncidents()
    {
        _logger.LogDebug("Navigating to IncidentList view");
        _navigationService.NavigateTo(NavigationConstants.IncidentList);
    }

    /// <summary>
    /// Command to view reports.
    /// </summary>
    [RelayCommand]
    private void ViewReports()
    {
        _logger.LogDebug("Navigating to Reports view");
        _navigationService.NavigateTo(NavigationConstants.Reports);
    }

    /// <summary>
    /// Command to refresh dashboard data.
    /// </summary>
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        IsLoading = true;
        _logger.LogDebug("Refreshing dashboard data");
        
        try
        {
            // Simulate data loading - replace with actual service calls
            await Task.Delay(1000);
            
            // TODO: Replace with actual data service calls
            await LoadDashboardDataAsync();
            
            LastRefreshTime = DateTime.Now;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing dashboard data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Initializes the ViewModel with sample data.
    /// </summary>
    private void InitializeSampleData()
    {
        ActiveIncidents = 12;
        HighPriorityIncidents = 3;
        ResolvedToday = 8;
        AverageResolutionTime = 4.2;
        LastRefreshTime = DateTime.Now;
    }

    /// <summary>
    /// Loads dashboard data from services.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LoadDashboardDataAsync()
    {
        // TODO: Implement actual data loading from services
        // This is a placeholder for future implementation
        
        await Task.Delay(100); // Simulate async operation
        
        // Sample data updates - replace with real service calls
        ActiveIncidents = Random.Shared.Next(5, 20);
        HighPriorityIncidents = Random.Shared.Next(0, 5);
        ResolvedToday = Random.Shared.Next(0, 15);
        AverageResolutionTime = Random.Shared.NextDouble() * 10;
    }

    /// <summary>
    /// Called when the view is loaded.
    /// </summary>
    public override async Task OnViewLoadedAsync()
    {
        _logger.LogDebug("Dashboard view loaded");
        await RefreshDataAsync();
    }
}
