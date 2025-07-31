using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Presentation.Constants;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the incident list view.
/// Handles data loading and user interactions for incident management.
/// Follows MVVM Community Toolkit patterns with async data loading.
/// </summary>
public partial class IncidentListViewModel : ObservableObject
{
    private readonly IIncidentService _incidentService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Collection of incidents to display in the UI.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<Incident> Incidents { get; set; } = new();

    /// <summary>
    /// Indicates whether data is currently being loaded.
    /// </summary>
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    /// <summary>
    /// Error message to display if data loading fails.
    /// </summary>
    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    /// <summary>
    /// Initializes a new instance of the IncidentListViewModel.
    /// </summary>
    /// <param name="incidentService">Service for incident data operations.</param>
    /// <param name="navigationService">Service for navigation operations.</param>
    public IncidentListViewModel(IIncidentService incidentService, INavigationService navigationService)
    {
        _incidentService = incidentService;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Loads incidents from the service asynchronously with cancellation support.
    /// Follows .NET 9/WinUI 3 async patterns with enhanced user experience.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true)]
    private async Task LoadIncidentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var incidentData = await _incidentService.GetAllAsync(cancellationToken);
            
            // Check for cancellation before UI updates
            cancellationToken.ThrowIfCancellationRequested();
            
            // Clear existing data and add new incidents
            Incidents.Clear();
            foreach (var incident in incidentData)
            {
                Incidents.Add(incident);
                
                // Allow cancellation during UI updates for large datasets
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully - don't show error for user-initiated cancellation
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load incidents: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refreshes the incident data by reloading from the service with cancellation support.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true)]
    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await LoadIncidentsAsync(cancellationToken);
    }

    /// <summary>
    /// Navigates to the incident creation page.
    /// </summary>
    [RelayCommand]
    private void CreateIncident()
    {
        _navigationService.NavigateTo(NavigationKeys.IncidentCreate);
    }
}
