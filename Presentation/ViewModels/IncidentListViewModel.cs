using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

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
    private ObservableCollection<Incident> incidents = new();

    /// <summary>
    /// Indicates whether data is currently being loaded.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Error message to display if data loading fails.
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

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
    /// Loads incidents from the service asynchronously.
    /// Follows Microsoft async/await patterns with proper error handling.
    /// </summary>
    [RelayCommand]
    private async Task LoadIncidentsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var incidentData = await _incidentService.GetAllAsync();
            
            // Clear existing data and add new incidents
            Incidents.Clear();
            foreach (var incident in incidentData)
            {
                Incidents.Add(incident);
            }
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
    /// Refreshes the incident data by reloading from the service.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadIncidentsAsync();
    }

    /// <summary>
    /// Navigates to the incident creation page.
    /// </summary>
    [RelayCommand]
    private void CreateIncident()
    {
        _navigationService.NavigateTo("IncidentCreate");
    }
}
