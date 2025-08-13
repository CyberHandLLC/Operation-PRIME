using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Presentation.Extensions;

namespace OperationPrime.Presentation.Helpers;

/// <summary>
/// Manages ObservableCollections for ViewModels to reduce repetitive collection management code.
/// Follows Single Responsibility Principle for collection operations.
/// </summary>
public class ViewModelCollectionsManager
{
    private readonly IEnumService _enumService;
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ViewModelCollectionsManager> _logger;

    public ViewModelCollectionsManager(
        IEnumService enumService,
        IApplicationService applicationService,
        ILogger<ViewModelCollectionsManager> logger)
    {
        _enumService = enumService;
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// All collections managed by this class.
    /// </summary>
    public ObservableCollection<IncidentType> IncidentTypes { get; } = new();
    public ObservableCollection<Priority> Priorities { get; } = new();
    public ObservableCollection<Status> Statuses { get; } = new();
    public ObservableCollection<ImpactedUsersCount> ImpactedUsersCounts { get; } = new();
    public ObservableCollection<IncidentSource> IncidentSources { get; } = new();
    public ObservableCollection<GeneratingMultipleCalls> GeneratingMultipleCallsOptions { get; } = new();
    public ObservableCollection<ApplicationInfo> Applications { get; } = new();

    /// <summary>
    /// Loads all collections in parallel for better performance.
    /// </summary>
    public async Task LoadAllCollectionsAsync()
    {
        _logger.LogDebug("Starting to load all ViewModel collections");
        
        try
        {
            // Load enum collections synchronously (fast)
            LoadEnumCollections();
            
            // Load applications asynchronously
            await LoadApplicationsAsync().ConfigureAwait(true);
            
            _logger.LogInformation("Successfully loaded all ViewModel collections");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ViewModel collections");
            throw;
        }
    }

    /// <summary>
    /// Loads enum collections using extension methods.
    /// </summary>
    private void LoadEnumCollections()
    {
        _logger.LogDebug("Loading enum collections");
        
        IncidentTypes.LoadFrom(_enumService.GetIncidentTypes());
        Priorities.LoadFrom(_enumService.GetPriorities());
        Statuses.LoadFrom(_enumService.GetStatuses());
        ImpactedUsersCounts.LoadFrom(_enumService.GetImpactedUsersCounts());
        IncidentSources.LoadFrom(_enumService.GetIncidentSources());
        GeneratingMultipleCallsOptions.LoadFrom(_enumService.GetGeneratingMultipleCallsOptions());
        
        _logger.LogDebug("Loaded {IncidentTypeCount} incident types, {PriorityCount} priorities, {StatusCount} statuses, {UserCountCount} user counts, {SourceCount} sources, {CallsFlagCount} multi-calls options",
            IncidentTypes.Count, Priorities.Count, Statuses.Count, ImpactedUsersCounts.Count, IncidentSources.Count, GeneratingMultipleCallsOptions.Count);
    }

    /// <summary>
    /// Loads applications for auto-suggestion.
    /// </summary>
    private async Task LoadApplicationsAsync()
    {
        _logger.LogDebug("Loading applications for auto-suggestion");
        
        try
        {
            var applications = await _applicationService.GetActiveApplicationsAsync().ConfigureAwait(true);
            Applications.LoadFrom(applications);
            
            _logger.LogInformation("Loaded {ApplicationCount} applications for auto-suggestion", Applications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load applications for auto-suggestion - continuing without auto-suggestion");
            // Don't rethrow - this is not critical for ViewModel operation
        }
    }
} 