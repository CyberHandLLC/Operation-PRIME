using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Presentation.Constants;
using OperationPrime.Presentation.ViewModels.Base;
using OperationPrime.Presentation.Extensions;
using OperationPrime.Presentation.Helpers;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for creating new incidents.
/// Refactored to follow Microsoft's 2024 MVVM Community Toolkit patterns with service extraction.
/// Business logic delegated to dedicated services for improved testability and maintainability.
/// </summary>
public partial class IncidentCreateViewModel : BaseValidatingViewModel
{
    private readonly IIncidentWorkflowService _workflowService;
    private readonly IIncidentOrchestrationService _orchestrationService;
    private readonly IIncidentValidationService _validationService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ViewModelCollectionsManager _collectionsManager;
    private readonly ILogger<IncidentCreateViewModel> _logger;

    /// <summary>
    /// Initializes a new instance of the IncidentCreateViewModel.
    /// ✅ CLEAN: Reduced dependencies using helper classes.
    /// </summary>
    public IncidentCreateViewModel(
        IIncidentWorkflowService workflowService,
        IIncidentOrchestrationService orchestrationService,
        IIncidentValidationService validationService,
        IDateTimeService dateTimeService,
        ViewModelCollectionsManager collectionsManager,
        ILogger<IncidentCreateViewModel> logger)
    {
        _workflowService = workflowService;
        _orchestrationService = orchestrationService;
        _validationService = validationService;
        _dateTimeService = dateTimeService;
        _collectionsManager = collectionsManager;
        _logger = logger;
        
        _logger.LogDebug("Initializing IncidentCreateViewModel");
        
        // Initialize with current Eastern Time (business timezone)
        var easternTime = _dateTimeService.GetCurrentEasternTime();
        TimeIssueStarted = easternTime;
        TimeReported = easternTime;
        
        // Load collections asynchronously
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initializes ViewModel asynchronously.
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            _logger.LogDebug("Starting ViewModel initialization");
            await _collectionsManager.LoadAllCollectionsAsync().ConfigureAwait(true);
            _logger.LogInformation("IncidentCreateViewModel initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize IncidentCreateViewModel");
            ErrorMessage = "Failed to load form data. Please refresh the page.";
        }
    }

    /// <summary>
    /// Title of the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident title is required")]
    [StringLength(ValidationLengths.TitleMaxLength, ErrorMessage = "Title cannot exceed {1} characters")]
    public partial string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident description is required")]
    [StringLength(ValidationLengths.DescriptionMaxLength, ErrorMessage = "Description cannot exceed {1} characters")]
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
    [StringLength(ValidationLengths.BusinessImpactMaxLength, ErrorMessage = "Business impact description cannot exceed {1} characters")]
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
    public partial DateTimeOffset? TimeIssueStarted { get; set; }

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial DateTimeOffset? TimeReported { get; set; }

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
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [StringLength(ValidationLengths.ApplicationAffectedMaxLength, ErrorMessage = "Application affected cannot exceed {1} characters")]
    public partial string ApplicationAffected { get; set; } = string.Empty;

    /// <summary>
    /// Physical or logical locations affected by the incident.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [StringLength(ValidationLengths.LocationsAffectedMaxLength, ErrorMessage = "Locations affected cannot exceed {1} characters")]
    public partial string LocationsAffected { get; set; } = string.Empty;

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(ValidationLengths.WorkaroundMaxLength, ErrorMessage = "Workaround cannot exceed {1} characters")]
    public partial string Workaround { get; set; } = string.Empty;

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyDataErrorInfo]
    [StringLength(ValidationLengths.IncidentNumberMaxLength, ErrorMessage = "Incident number cannot exceed {1} characters")]
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

    // ✅ CLEAN: Computed Properties using Helper Classes (Reduced Complexity)
    public bool IsMajorIncident => IncidentType == IncidentType.MajorIncident;
    public bool IsStep1 => StepNavigationHelper.IsStep(CurrentStep, 1);
    public bool IsStep2 => StepNavigationHelper.IsStep(CurrentStep, 2);
    public bool IsStep3 => StepNavigationHelper.IsStep(CurrentStep, 3);
    public bool IsStep4 => StepNavigationHelper.IsStep(CurrentStep, 4) && IsMajorIncident;
    public int TotalSteps => StepNavigationHelper.GetTotalSteps(IncidentType);
    public bool ShowNextButton => StepNavigationHelper.ShouldShowNextButton(CurrentStep, IncidentType);
    public bool CanGoNext => _workflowService.CanGoNext(CurrentStep, IncidentType, IsCurrentStepValid()) && CurrentStep < TotalSteps;
    public bool CanGoPrevious => CurrentStep > 1;
    public bool IsLastStep => StepNavigationHelper.IsLastStep(CurrentStep, IncidentType);

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

    public int CurrentBreadcrumbIndex => StepNavigationHelper.GetBreadcrumbIndex(CurrentStep);

    // ✅ CLEAN: Delegated Collection Properties (Managed by Helper)
    public ObservableCollection<IncidentType> AvailableIncidentTypes => _collectionsManager.IncidentTypes;
    public ObservableCollection<Priority> AvailablePriorities => _collectionsManager.Priorities;
    public ObservableCollection<Status> AvailableStatuses => _collectionsManager.Statuses;
    public ObservableCollection<ImpactedUsersCount> AvailableImpactedUsersCounts => _collectionsManager.ImpactedUsersCounts;
    public ObservableCollection<ApplicationInfo> AvailableApplications => _collectionsManager.Applications;

    /// <summary>
    /// Creates a new incident using the orchestration service.
    /// ✅ CLEAN: ViewModel only handles UI state, business logic delegated to services.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreateIncident), IncludeCancelCommand = true)]
    private async Task CreateIncidentAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting incident creation process");
        
        try
        {
            IsSubmitting = true;
            ClearMessages();

            var formData = this.ToFormData();
            _logger.LogDebug("Created form data for incident: {IncidentType} - {Title}", formData.IncidentType, formData.Title);

            var result = await _orchestrationService.CreateIncidentAsync(formData, cancellationToken).ConfigureAwait(true);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Incident created successfully: {IncidentId}", result.Value?.Id);
                SuccessMessage = "Incident created successfully!";
                ResetForm();
            }
            else
            {
                var errorMsg = result.ValidationErrors.Count > 0 ? result.ValidationErrors.First() : result.ErrorMessage ?? "Failed to create incident";
                _logger.LogWarning("Incident creation failed: {ErrorMessage}", errorMsg);
                ErrorMessage = errorMsg;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Incident creation was cancelled by user");
            ClearMessages();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during incident creation");
            ErrorMessage = $"An unexpected error occurred: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
            _logger.LogDebug("Incident creation process completed, IsSubmitting set to false");
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
            CurrentStep = _workflowService.GetNextStep(CurrentStep, IncidentType);
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
            CurrentStep = _workflowService.GetPreviousStep(CurrentStep);
            ErrorMessage = null;
        }
    }

    /// <summary>
    /// Navigates directly to a specific step (for breadcrumb navigation).
    /// </summary>
    [RelayCommand]
    private void GoToStep(int step)
    {
        if (StepNavigationHelper.IsValidStep(step, IncidentType))
        {
            _logger.LogDebug("Navigating to step {Step} for {IncidentType}", step, IncidentType);
            CurrentStep = step;
            ErrorMessage = null;
        }
        else
        {
            _logger.LogWarning("Invalid step navigation attempt: step {Step} for {IncidentType}", step, IncidentType);
        }
    }

    /// <summary>
    /// Selects Pre-Incident type and advances to step 2.
    /// ✅ CLEAN: Simplified property setting with automatic navigation.
    /// </summary>
    [RelayCommand]
    private void SelectPreIncident()
    {
        _logger.LogDebug("Selecting Pre-Incident type and advancing to step 2");
        IncidentType = IncidentType.PreIncident;
        CurrentStep = _workflowService.GetNextStep(1, IncidentType.PreIncident);
        ErrorMessage = null;
    }

    /// <summary>
    /// Selects Major Incident type and advances to step 2.
    /// ✅ CLEAN: Simplified property setting with automatic navigation.
    /// </summary>
    [RelayCommand]
    private void SelectMajorIncident()
    {
        _logger.LogDebug("Selecting Major Incident type and advancing to step 2");
        IncidentType = IncidentType.MajorIncident;
        CurrentStep = _workflowService.GetNextStep(1, IncidentType.MajorIncident);
        ErrorMessage = null;
    }

    /// <summary>
    /// Resets the form to default values and returns to Step 1.
    /// ✅ CLEAN: Direct property reset - simple and efficient.
    /// </summary>
    [RelayCommand]
    private void ResetForm()
    {
        ClearErrors();
        ClearMessages();
        
        // Reset to default values
        IncidentType = IncidentType.PreIncident;
        Priority = Priority.P3;
        Status = Status.New;
        Urgency = 3;
        CurrentStep = 1;
        
        // Clear all form fields
        Title = Description = BusinessImpact = string.Empty;
        ApplicationAffected = LocationsAffected = Workaround = IncidentNumber = string.Empty;
        
        // Reset dates to current Eastern Time
        var easternTime = _dateTimeService.GetCurrentEasternTime();
        TimeIssueStarted = TimeReported = easternTime;
        
        _logger.LogDebug("ViewModel Reset: TimeIssueStarted={TimeIssueStarted}, TimeReported={TimeReported}", 
            TimeIssueStarted, TimeReported);
        
        // Clear user count
        ImpactedUsers = null;
        SelectedImpactedUsersCount = null;
    }

    /// <summary>
    /// Determines if the incident can be created (all required fields are valid).
    /// ✅ CLEAN: ViewModel coordinates UI state, validation delegated to orchestration service.
    /// </summary>
    private bool CanCreateIncident()
    {
        _logger.LogDebug("CanCreateIncident check: CurrentStep={CurrentStep}, IsLastStep={IsLastStep}, IsSubmitting={IsSubmitting}, IncidentType={IncidentType}", 
            CurrentStep, IsLastStep, IsSubmitting, IncidentType);
        
        // Must be on the last step
        if (!IsLastStep) 
        {
            _logger.LogDebug("Cannot create incident: not on last step");
            return false;
        }
        
        // Not submitting already
        if (IsSubmitting) 
        {
            _logger.LogDebug("Cannot create incident: already submitting");
            return false;
        }
        
        // ✅ CLEAN: Use orchestration service for comprehensive validation
        var formData = this.ToFormData();
        var validationResult = _orchestrationService.ValidateIncidentData(formData);
        var isValid = validationResult.IsValid;
        _logger.LogDebug("Form validation result: {IsValid}. Errors: {Errors}", isValid, string.Join(", ", validationResult.Errors));
        return isValid;
    }

    /// <summary>
    /// Updates the TimeIssueStarted using business logic from DateTimeService.
    /// Called by UI layer when date/time controls change.
    /// </summary>
    public void UpdateTimeIssueStarted(DateTimeOffset date, TimeSpan time)
    {
        TimeIssueStarted = _dateTimeService.CombineDateAndTime(date, time);
    }

    /// <summary>
    /// Clears the TimeIssueStarted value.
    /// Called by UI layer when date/time controls are cleared.
    /// </summary>
    public void ClearTimeIssueStarted()
    {
        TimeIssueStarted = null;
    }

    /// <summary>
    /// Updates the TimeReported using business logic from DateTimeService.
    /// Called by UI layer when date/time controls change.
    /// </summary>
    public void UpdateTimeReported(DateTimeOffset date, TimeSpan time)
    {
        TimeReported = _dateTimeService.CombineDateAndTime(date, time);
    }

    /// <summary>
    /// Clears the TimeReported value.
    /// Called by UI layer when date/time controls are cleared.
    /// </summary>
    public void ClearTimeReported()
    {
        TimeReported = null;
    }

    /// <summary>
    /// Validates the current step's required fields using the validation service.
    /// ✅ CLEAN: Uses service layer for validation, maintaining clean architecture.
    /// </summary>
    private bool IsCurrentStepValid()
    {
        _logger.LogDebug("Validating step {CurrentStep} for {IncidentType}", CurrentStep, IncidentType);
        
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

}