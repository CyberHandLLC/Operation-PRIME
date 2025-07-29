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
    /// Initializes a new instance of the IncidentCreateViewModel.
    /// </summary>
    /// <param name="incidentService">Service for incident operations.</param>
    /// <param name="enumService">Service for enum collections.</param>
    public IncidentCreateViewModel(IIncidentService incidentService, IEnumService enumService)
    {
        _incidentService = incidentService;
        _enumService = enumService;
        
        // Load enum collections first (required for proper validation)
        LoadEnumCollections();
        
        // Initialize validation state for all default values
        // This ensures buttons are enabled/disabled correctly from the start
        RefreshValidationState();
    }

    /// <summary>
    /// Title of the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private string title = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private string description = string.Empty;

    /// <summary>
    /// Type of incident being created.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMajorIncident))]
    [NotifyPropertyChangedFor(nameof(IsPreIncidentSelected))]
    [NotifyPropertyChangedFor(nameof(IsMajorIncidentSelected))]
    [NotifyPropertyChangedFor(nameof(TotalSteps))]
    [NotifyPropertyChangedFor(nameof(IsStep4))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private IncidentType incidentType = IncidentType.PreIncident;

    /// <summary>
    /// Indicates whether Pre-Incident type is selected.
    /// </summary>
    public bool IsPreIncidentSelected => IncidentType == IncidentType.PreIncident;

    /// <summary>
    /// Indicates whether Major Incident type is selected.
    /// </summary>
    public bool IsMajorIncidentSelected => IncidentType == IncidentType.MajorIncident;

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
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
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
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
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
    [NotifyPropertyChangedFor(nameof(ShowNextButton))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private int currentStep = 1;

    // New Fields for Step-by-Step Workflow
    /// <summary>
    /// Date and time when the issue started.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private DateTimeOffset? timeIssueStarted = DateTimeOffset.Now;

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private DateTimeOffset? timeReported = DateTimeOffset.Now;

    /// <summary>
    /// Description of users impacted by the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private string impactedUsers = string.Empty;

    /// <summary>
    /// Application or system affected by the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private string applicationAffected = string.Empty;

    /// <summary>
    /// Physical or logical locations affected by the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private string locationsAffected = string.Empty;

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
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
    [NotifyPropertyChangedFor(nameof(UrgencyIndex))]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    private int urgency = 3;

    /// <summary>
    /// Gets or sets the urgency index for ComboBox binding (0-based).
    /// </summary>
    public int UrgencyIndex
    {
        get => Urgency - 1; // Convert 1-based to 0-based
        set => Urgency = value + 1; // Convert 0-based to 1-based
    }

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
    /// Gets whether the Next button should be shown (Steps 2-3 only, Step 1 auto-advances).
    /// </summary>
    public bool ShowNextButton => CurrentStep >= 2 && !IsLastStep;

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
    [RelayCommand(CanExecute = nameof(CanCreateIncident))]
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
    /// Selects Pre-Incident type and updates the workflow accordingly.
    /// </summary>
    [RelayCommand]
    private void SelectPreIncident()
    {
        IncidentType = IncidentType.PreIncident;
        ErrorMessage = null;
        
        // Clear Major Incident specific fields when switching to Pre-Incident
        BusinessImpact = string.Empty;
        
        // If we're past step 3 and switching to Pre-Incident, 
        // move to step 3 (final step for Pre-Incident)
        if (CurrentStep > 3)
        {
            CurrentStep = 3;
        }
        
        // Auto-advance to next step for better UX (only from step 1)
        if (CurrentStep == 1)
        {
            GoToNextStep();
        }
    }

    /// <summary>
    /// Selects Major Incident type and updates the workflow accordingly.
    /// </summary>
    [RelayCommand]
    private void SelectMajorIncident()
    {
        IncidentType = IncidentType.MajorIncident;
        ErrorMessage = null;
        
        // No need to clear fields when switching to Major Incident
        // (BusinessImpact field will be available in Step 4)
        
        // Auto-advance to next step for better UX (only from step 1)
        if (CurrentStep == 1)
        {
            GoToNextStep();
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
    /// Determines if the incident can be created (all required fields are valid).
    /// </summary>
    private bool CanCreateIncident()
    {
        // Must complete all steps for the current incident type
        if (!IsLastStep) return false;
        
        // Validate all steps
        for (int step = 1; step <= TotalSteps; step++)
        {
            var isValid = step switch
            {
                1 => IncidentType != default, // Step 1: Incident Type selected
                2 => ValidateStep2(), // Step 2: Basic Information
                3 => ValidateStep3(), // Step 3: Incident Details
                4 => ValidateStep4(), // Step 4: Master Checklist (Major Incidents only)
                _ => false
            };
            
            if (!isValid) return false;
        }
        
        return !IsSubmitting;
    }

    /// <summary>
    /// Refreshes the validation state by triggering property change notifications.
    /// This ensures buttons are properly enabled/disabled based on current form state.
    /// </summary>
    private void RefreshValidationState()
    {
        // Trigger property change notifications for validation-dependent properties
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(IsLastStep));
        OnPropertyChanged(nameof(ShowNextButton));
        
        // Trigger command CanExecute refresh
        GoToNextStepCommand.NotifyCanExecuteChanged();
        CreateIncidentCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Resets the form to default values and returns to Step 1.
    /// Clears all data for a fresh start.
    /// </summary>
    [RelayCommand]
    private void ResetForm()
    {
        // Reset basic incident fields
        Title = string.Empty;
        Description = string.Empty;
        BusinessImpact = string.Empty;
        IncidentType = IncidentType.PreIncident; // Default to Pre-Incident
        Priority = Priority.P3;
        Status = Status.New;
        
        // Reset step-specific fields (use current date/time as defaults)
        TimeIssueStarted = DateTimeOffset.Now;
        TimeReported = DateTimeOffset.Now;
        ImpactedUsers = string.Empty;
        ApplicationAffected = string.Empty;
        LocationsAffected = string.Empty;
        Workaround = string.Empty;
        IncidentNumber = string.Empty;
        Urgency = 3; // Default to Low urgency
        
        // Reset navigation state - return to Step 1
        CurrentStep = 1;
        
        // Clear any messages
        ErrorMessage = null;
        SuccessMessage = null;
        
        // Refresh validation state after reset
        RefreshValidationState();
    }
}
