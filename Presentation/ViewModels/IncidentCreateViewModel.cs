using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for creating new incidents.
/// Matches the database schema: Title, Description, IncidentType, Priority, Status, CreatedDate.
/// </summary>
public partial class IncidentCreateViewModel : ObservableObject
{
    private readonly IIncidentService _incidentService;
    private readonly IEnumService _enumService;

    /// <summary>
    /// Title of the incident.
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Type of incident being created.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMajorIncident))]
    private IncidentType incidentType = IncidentType.PreIncident;

    /// <summary>
    /// Priority level of the incident.
    /// </summary>
    [ObservableProperty]
    private Priority priority = Priority.P3;

    /// <summary>
    /// Current status of the incident.
    /// </summary>
    [ObservableProperty]
    private Status status = Status.New;

    /// <summary>
    /// Indicates whether the form is currently being submitted.
    /// </summary>
    [ObservableProperty]
    private bool isSubmitting;

    /// <summary>
    /// Error message to display if creation fails.
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// Success message to display when incident is created.
    /// </summary>
    [ObservableProperty]
    private string? successMessage;

    /// <summary>
    /// Business impact description for Major Incidents.
    /// Describes what users cannot do due to the incident.
    /// </summary>
    [ObservableProperty]
    private string businessImpact = string.Empty;

    // Step Navigation Properties
    /// <summary>
    /// Current step in the incident creation workflow (1-4).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStep1))]
    [NotifyPropertyChangedFor(nameof(IsStep2))]
    [NotifyPropertyChangedFor(nameof(IsStep3))]
    [NotifyPropertyChangedFor(nameof(IsStep4))]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(CanGoPrevious))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyPropertyChangedFor(nameof(TotalSteps))]
    [NotifyPropertyChangedFor(nameof(CurrentBreadcrumbIndex))]
    private int currentStep = 1;

    // New Fields for Step-by-Step Workflow
    /// <summary>
    /// Date and time when the issue started.
    /// </summary>
    [ObservableProperty]
    private DateTime? timeIssueStarted;

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    [ObservableProperty]
    private DateTime? timeReported;

    /// <summary>
    /// Description of users impacted by the incident.
    /// </summary>
    [ObservableProperty]
    private string impactedUsers = string.Empty;

    /// <summary>
    /// Application(s) affected by the incident.
    /// </summary>
    [ObservableProperty]
    private string applicationAffected = string.Empty;

    /// <summary>
    /// Location(s) affected by the incident.
    /// </summary>
    [ObservableProperty]
    private string locationsAffected = string.Empty;

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    [ObservableProperty]
    private string workaround = string.Empty;

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    [ObservableProperty]
    private string incidentNumber = string.Empty;

    /// <summary>
    /// Urgency level of the incident (1=High, 2=Medium, 3=Low).
    /// </summary>
    [ObservableProperty]
    private int urgency = 3;

    // Computed Properties for Step Navigation
    /// <summary>
    /// Computed property to determine if the current incident type is Major Incident.
    /// Used for conditional UI display of Major Incident specific fields.
    /// </summary>
    public bool IsMajorIncident => IncidentType == IncidentType.MajorIncident;

    /// <summary>
    /// Gets whether the current step is Step 1 (Incident Type).
    /// </summary>
    public bool IsStep1 => CurrentStep == 1;

    /// <summary>
    /// Gets whether the current step is Step 2 (Basic Information).
    /// </summary>
    public bool IsStep2 => CurrentStep == 2;

    /// <summary>
    /// Gets whether the current step is Step 3 (Incident Details).
    /// </summary>
    public bool IsStep3 => CurrentStep == 3;

    /// <summary>
    /// Gets whether the current step is Step 4 (Master Checklist - Major Incidents only).
    /// </summary>
    public bool IsStep4 => CurrentStep == 4 && IsMajorIncident;

    /// <summary>
    /// Gets the total number of steps based on incident type.
    /// </summary>
    public int TotalSteps => IsMajorIncident ? 4 : 3;

    /// <summary>
    /// Gets whether the user can navigate to the next step.
    /// </summary>
    public bool CanGoNext => ValidateCurrentStep() && CurrentStep < TotalSteps;

    /// <summary>
    /// Gets whether the user can navigate to the previous step.
    /// </summary>
    public bool CanGoPrevious => CurrentStep > 1;

    /// <summary>
    /// Gets whether the current step is the last step.
    /// </summary>
    public bool IsLastStep => CurrentStep == TotalSteps;

    /// <summary>
    /// Gets the breadcrumb items for navigation.
    /// </summary>
    public ObservableCollection<string> BreadcrumbItems { get; } = new()
    {
        "Type",
        "Basic Info", 
        "Details",
        "Checklist"
    };

    /// <summary>
    /// Gets the current breadcrumb index (0-based for ItemsSource).
    /// </summary>
    public int CurrentBreadcrumbIndex => CurrentStep - 1;

    /// <summary>
    /// Collection of available incident types for the ComboBox.
    /// Populated from Domain Service following Clean Architecture principles.
    /// </summary>
    public ObservableCollection<IncidentType> AvailableIncidentTypes { get; private set; } = new();

    /// <summary>
    /// Collection of available priority levels for the ComboBox.
    /// Populated from Domain Service following Clean Architecture principles.
    /// </summary>
    public ObservableCollection<Priority> AvailablePriorities { get; private set; } = new();

    /// <summary>
    /// Collection of available status values for the ComboBox.
    /// Populated from Domain Service following Clean Architecture principles.
    /// </summary>
    public ObservableCollection<Status> AvailableStatuses { get; private set; } = new();

    /// <summary>
    /// Initializes a new instance of the IncidentCreateViewModel.
    /// </summary>
    /// <param name="incidentService">Service for incident data operations.</param>
    /// <param name="enumService">Service for enum collections following Clean Architecture.</param>
    public IncidentCreateViewModel(IIncidentService incidentService, IEnumService enumService)
    {
        _incidentService = incidentService;
        _enumService = enumService;
        
        // Populate enum collections from domain service
        LoadEnumCollections();
    }

    /// <summary>
    /// Loads enum collections from the domain service.
    /// Follows Clean Architecture principles by delegating to domain layer.
    /// </summary>
    private void LoadEnumCollections()
    {
        // Clear existing collections
        AvailableIncidentTypes.Clear();
        AvailablePriorities.Clear();
        AvailableStatuses.Clear();
        
        // Populate from domain service
        foreach (var type in _enumService.GetIncidentTypes())
        {
            AvailableIncidentTypes.Add(type);
        }
        
        foreach (var priority in _enumService.GetPriorities())
        {
            AvailablePriorities.Add(priority);
        }
        
        foreach (var status in _enumService.GetStatuses())
        {
            AvailableStatuses.Add(status);
        }
    }

    /// <summary>
    /// Creates a new incident with the current form data.
    /// </summary>
    [RelayCommand]
    private async Task CreateIncidentAsync()
    {
        try
        {
            IsSubmitting = true;
            ErrorMessage = null;
            SuccessMessage = null;

            // Basic validation
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Title is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                ErrorMessage = "Description is required.";
                return;
            }

            // Major Incident specific validation
            if (IncidentType == IncidentType.MajorIncident && string.IsNullOrWhiteSpace(BusinessImpact))
            {
                ErrorMessage = "Business Impact is required for Major Incidents.";
                return;
            }

            // Generate incident number if not provided
            if (string.IsNullOrWhiteSpace(IncidentNumber))
            {
                IncidentNumber = $"INC-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks % 10000:D4}";
            }

            // Create the incident entity - matching database schema exactly
            var incident = new Incident
            {
                Title = Title,
                Description = Description,
                BusinessImpact = IncidentType == IncidentType.MajorIncident ? BusinessImpact : null,
                TimeIssueStarted = TimeIssueStarted,
                TimeReported = TimeReported,
                ImpactedUsers = ImpactedUsers,
                ApplicationAffected = ApplicationAffected,
                LocationsAffected = LocationsAffected,
                Workaround = string.IsNullOrWhiteSpace(Workaround) ? null : Workaround,
                IncidentNumber = IncidentNumber,
                Urgency = Urgency,
                IncidentType = IncidentType,
                Priority = Priority,
                Status = Status,
                CreatedDate = DateTime.UtcNow
            };

            // Save the incident
            await _incidentService.CreateAsync(incident);

            SuccessMessage = "Incident created successfully!";
            
            // Reset form for next incident
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create incident: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    /// <summary>
    /// Navigates to the next step in the workflow.
    /// </summary>
    [RelayCommand]
    private void GoToNextStep()
    {
        if (CanGoNext)
        {
            CurrentStep++;
            ErrorMessage = null;
        }
    }

    /// <summary>
    /// Navigates to the previous step in the workflow.
    /// </summary>
    [RelayCommand]
    private void GoToPreviousStep()
    {
        if (CanGoPrevious)
        {
            CurrentStep--;
            ErrorMessage = null;
        }
    }

    /// <summary>
    /// Navigates directly to a specific step (for breadcrumb navigation).
    /// </summary>
    /// <param name="step">The step number to navigate to.</param>
    [RelayCommand]
    private void GoToStep(int step)
    {
        if (step >= 1 && step <= TotalSteps && step <= CurrentStep + 1)
        {
            CurrentStep = step;
            ErrorMessage = null;
        }
    }

    /// <summary>
    /// Validates the current step to determine if user can proceed.
    /// </summary>
    private bool ValidateCurrentStep()
    {
        return CurrentStep switch
        {
            1 => IncidentType != default, // Step 1: Incident Type selected
            2 => ValidateStep2(), // Step 2: Basic Information
            3 => ValidateStep3(), // Step 3: Incident Details
            4 => ValidateStep4(), // Step 4: Master Checklist (Major Incidents only)
            _ => false
        };
    }

    /// <summary>
    /// Validates Step 2 (Basic Information) fields.
    /// </summary>
    private bool ValidateStep2()
    {
        return TimeIssueStarted.HasValue &&
               TimeReported.HasValue &&
               !string.IsNullOrWhiteSpace(ImpactedUsers) &&
               !string.IsNullOrWhiteSpace(ApplicationAffected) &&
               !string.IsNullOrWhiteSpace(LocationsAffected);
        // Workaround is optional
    }

    /// <summary>
    /// Validates Step 3 (Incident Details) fields.
    /// </summary>
    private bool ValidateStep3()
    {
        return !string.IsNullOrWhiteSpace(Title) &&
               !string.IsNullOrWhiteSpace(Description) &&
               Urgency >= 1 && Urgency <= 3;
        // IncidentNumber can be auto-generated
    }

    /// <summary>
    /// Validates Step 4 (Master Checklist) fields for Major Incidents.
    /// </summary>
    private bool ValidateStep4()
    {
        // Only required for Major Incidents
        return !IsMajorIncident || !string.IsNullOrWhiteSpace(BusinessImpact);
    }

    /// <summary>
    /// Resets the form to default values.
    /// </summary>
    [RelayCommand]
    private void ResetForm()
    {
        // Reset basic fields
        Title = string.Empty;
        Description = string.Empty;
        BusinessImpact = string.Empty;
        IncidentType = IncidentType.PreIncident;
        Priority = Priority.P3;
        Status = Status.New;
        
        // Reset new step fields
        TimeIssueStarted = null;
        TimeReported = null;
        ImpactedUsers = string.Empty;
        ApplicationAffected = string.Empty;
        LocationsAffected = string.Empty;
        Workaround = string.Empty;
        IncidentNumber = string.Empty;
        Urgency = 3;
        
        // Reset step navigation
        CurrentStep = 1;
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
