using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Presentation.ViewModels.Base;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for creating new incidents.
/// Refactored to follow Microsoft's 2024 MVVM Community Toolkit patterns with service extraction.
/// Business logic delegated to dedicated services for improved testability and maintainability.
/// </summary>
public partial class IncidentCreateViewModel : BaseValidatingViewModel
{
    private readonly IIncidentService _incidentService;
    private readonly IEnumService _enumService;
    private readonly IApplicationService _applicationService;
    private readonly IIncidentValidationService _validationService;

    /// <summary>
    /// Initializes a new instance of the IncidentCreateViewModel.
    /// </summary>
    /// <param name="incidentService">Service for incident operations.</param>
    /// <param name="enumService">Service for enum collections.</param>
    /// <param name="applicationService">Service for application auto-suggestion.</param>
    /// <param name="validationService">Service for validation logic.</param>
    public IncidentCreateViewModel(IIncidentService incidentService, IEnumService enumService, IApplicationService applicationService, IIncidentValidationService validationService)
    {
        _incidentService = incidentService;
        _enumService = enumService;
        _applicationService = applicationService;
        _validationService = validationService;
        
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
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public partial string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident description is required")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public partial string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of incident being created.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMajorIncident), nameof(IsPreIncidentSelected), nameof(IsMajorIncidentSelected), nameof(TotalSteps), nameof(IsStep4))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial IncidentType IncidentType { get; set; } = IncidentType.PreIncident;

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
    public partial Priority Priority { get; set; } = Priority.P3;

    /// <summary>
    /// Current status of the incident.
    /// </summary>
    [ObservableProperty]
    public partial Status Status { get; set; } = Status.New;

    /// <summary>
    /// Indicates whether the form is currently being submitted.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial bool IsSubmitting { get; set; }



    /// <summary>
    /// Business impact description for Major Incidents.
    /// Describes what users cannot do due to the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [StringLength(1000, ErrorMessage = "Business impact description cannot exceed 1000 characters")]
    public partial string BusinessImpact { get; set; } = string.Empty;

    // Step Navigation Properties
    /// <summary>
    /// Current step in the incident creation workflow (1-4).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStep1), nameof(IsStep2), nameof(IsStep3), nameof(IsStep4), nameof(CanGoNext), nameof(CanGoPrevious), nameof(IsLastStep), nameof(TotalSteps), nameof(CurrentBreadcrumbIndex), nameof(ShowNextButton))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial int CurrentStep { get; set; } = 1;

    // New Fields for Step-by-Step Workflow
    /// <summary>
    /// Date and time when the issue started.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial DateTimeOffset? TimeIssueStarted { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial DateTimeOffset? TimeReported { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Number of users impacted by the incident.
    /// Selected from predefined values for consistent reporting.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial int? ImpactedUsers { get; set; }
    
    /// <summary>
    /// Selected impacted users count enum for ComboBox binding.
    /// Automatically syncs with ImpactedUsers integer value.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial ImpactedUsersCount? SelectedImpactedUsersCount { get; set; }
    
    /// <summary>
    /// Handles changes to SelectedImpactedUsersCount and syncs with ImpactedUsers.
    /// </summary>
    partial void OnSelectedImpactedUsersCountChanged(ImpactedUsersCount? value)
    {
        ImpactedUsers = value.HasValue ? (int)value.Value : null;
    }
    
    /// <summary>
    /// Handles changes to ImpactedUsers and syncs with SelectedImpactedUsersCount.
    /// </summary>
    partial void OnImpactedUsersChanged(int? value)
    {
        if (value.HasValue && Enum.IsDefined(typeof(ImpactedUsersCount), value.Value))
        {
            SelectedImpactedUsersCount = (ImpactedUsersCount)value.Value;
        }
        else
        {
            SelectedImpactedUsersCount = null;
        }
    }

    /// <summary>
    /// Application(s) affected by the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial string ApplicationAffected { get; set; } = string.Empty;

    /// <summary>
    /// Physical or logical locations affected by the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial string LocationsAffected { get; set; } = string.Empty;

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial string Workaround { get; set; } = string.Empty;

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    [ObservableProperty]
    public partial string IncidentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Urgency level of the incident (1=High, 2=Medium, 3=Low).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UrgencyIndex), nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial int Urgency { get; set; } = 3;

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
    /// Collection of available impacted users count values for the ComboBox.
    /// Populated from Domain Service following Clean Architecture principles.
    /// </summary>
    public ObservableCollection<ImpactedUsersCount> AvailableImpactedUsersCounts { get; private set; } = new();
    
    /// <summary>
    /// Collection of available applications for auto-suggestion.
    /// Populated from Application Service.
    /// </summary>
    public ObservableCollection<ApplicationInfo> AvailableApplications { get; private set; } = new();

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
        AvailableImpactedUsersCounts.Clear();
        
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
        
        foreach (var count in _enumService.GetImpactedUsersCounts())
        {
            AvailableImpactedUsersCounts.Add(count);
        }
        
        // Load applications asynchronously
        _ = LoadApplicationsAsync();
    }
    
    /// <summary>
    /// Loads applications for auto-suggestion from the application service.
    /// </summary>
    private async Task LoadApplicationsAsync()
    {
        try
        {
            var applications = await _applicationService.GetActiveApplicationsAsync();
            
            AvailableApplications.Clear();
            foreach (var app in applications)
            {
                AvailableApplications.Add(app);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire ViewModel initialization
            ErrorMessage = $"Failed to load applications: {ex.Message}";
        }
    }

    /// <summary>
    /// Creates a new incident with the current form data with cancellation support.
    /// Follows .NET 9/WinUI 3 enhanced async command patterns.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreateIncident), IncludeCancelCommand = true)]
    private async Task CreateIncidentAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsSubmitting = true;
            ErrorMessage = null;
            SuccessMessage = null;

            // Comprehensive validation using data annotations
            ValidateAllProperties();
        
            // Check for validation errors and provide specific feedback
            if (HasErrors)
            {
                // Get the first validation error to display to user
                var firstError = GetErrors().FirstOrDefault();
                ErrorMessage = firstError?.ErrorMessage ?? "Please correct the validation errors and try again.";
                return;
            }
        
            // Major Incident specific validation (business rule)
            if (IncidentType == IncidentType.MajorIncident && string.IsNullOrWhiteSpace(BusinessImpact))
            {
                ErrorMessage = "Business Impact is required for Major Incidents.";
                return;
            }

            // Generate incident number if not provided
            if (string.IsNullOrWhiteSpace(IncidentNumber))
            {
                IncidentNumber = $"INC-{DateTimeOffset.UtcNow:yyyyMMdd}-{DateTimeOffset.UtcNow.Ticks % 10000:D4}";
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
                CreatedDate = DateTimeOffset.UtcNow
            };

            // Check for cancellation before saving
            cancellationToken.ThrowIfCancellationRequested();
        
            // Save the incident
            await _incidentService.CreateAsync(incident, cancellationToken);
        
            // Check for cancellation before UI updates
            cancellationToken.ThrowIfCancellationRequested();

            SuccessMessage = "Incident created successfully!";
        
            // Reset form for next incident
            ResetForm();
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully - don't show error for user-initiated cancellation
            ErrorMessage = null;
            SuccessMessage = null;
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
    /// Validates the current step using the refactored validation service.
    /// Modern Microsoft 2024 MVVM Community Toolkit approach with service delegation.
    /// </summary>
    private bool ValidateCurrentStep()
    {
        return CurrentStep switch
        {
            1 => _validationService.ValidateStep1(IncidentType),
            2 => _validationService.ValidateStep2(ImpactedUsers, Urgency, BusinessImpact, 
                    ApplicationAffected, LocationsAffected, Workaround),
            3 => _validationService.ValidateStep3(IncidentNumber, Title, Description, 
                    TimeIssueStarted, TimeReported),
            4 => _validationService.ValidateStep4(IncidentType, Priority, Status),
            _ => false
        };
    }

    /// <summary>
    /// Determines if the incident can be created (all required fields are valid).
    /// Modern Microsoft 2024 MVVM Community Toolkit approach using refactored validation service.
    /// </summary>
    private bool CanCreateIncident()
    {
        // Must be on the last step
        if (!IsLastStep) return false;
        
        // Not submitting already
        if (IsSubmitting) return false;
        
        // Use refactored validation service for comprehensive validation
        return _validationService.ValidateAllProperties(IncidentType, IncidentNumber, Title, Description,
            ImpactedUsers, Urgency, BusinessImpact, ApplicationAffected,
            LocationsAffected, Workaround, TimeIssueStarted, TimeReported);
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
    /// </summary>
    [RelayCommand]
    private void ResetForm()
    {
        // Clear all validation errors first
        ClearErrors();
        
        // Reset text fields
        Title = Description = BusinessImpact = ApplicationAffected = 
        LocationsAffected = Workaround = IncidentNumber = string.Empty;
        
        // Reset enums to defaults
        IncidentType = IncidentType.PreIncident;
        Priority = Priority.P3;
        Status = Status.New;
        Urgency = 3;
        
        // Reset date/time fields to current time
        TimeIssueStarted = TimeReported = DateTimeOffset.UtcNow;
        
        // Reset nullable fields
        ImpactedUsers = null;
        SelectedImpactedUsersCount = null;
        ErrorMessage = SuccessMessage = null;
        
        // Reset navigation
        CurrentStep = 1;
        
        // Clear errors again after reset and refresh validation state
        ClearErrors();
        RefreshValidationState();
    }
}
