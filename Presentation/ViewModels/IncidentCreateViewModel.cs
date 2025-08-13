using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Domain.Constants;
using OperationPrime.Presentation.ViewModels.Base;
using OperationPrime.Presentation.Extensions;
using OperationPrime.Presentation.Helpers;


namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for creating new incidents.
/// Refactored to follow Microsoft's 2024 MVVM Community Toolkit patterns with service extraction.
/// Business logic delegated to dedicated services for improved testability and maintainability.
/// Implements async initialization pattern following 2024 best practices.
/// </summary>
public partial class IncidentCreateViewModel : BaseValidatingViewModel, IAsyncInitializable
{
    private readonly IIncidentWorkflowService _workflowService;
    private readonly IIncidentOrchestrationService _orchestrationService;
    private readonly IIncidentValidationService _validationService;
    private readonly IIncidentDataMappingService _dataMappingService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ViewModelCollectionsManager _collectionsManager;
    private readonly ILogger<IncidentCreateViewModel> _logger;

    /// <summary>
    /// Indicates whether the ViewModel has been initialized.
    /// Part of IAsyncInitializable interface following 2024 best practices.
    /// </summary>
    [ObservableProperty]
    public partial bool IsInitialized { get; set; }

    /// <summary>
    /// Initializes a new instance of the IncidentCreateViewModel.
    /// ✅ IMPROVED: Fast constructor following 2024 best practices - no async operations.
    /// </summary>
    public IncidentCreateViewModel(
        IIncidentWorkflowService workflowService,
        IIncidentOrchestrationService orchestrationService,
        IIncidentValidationService validationService,
        IIncidentDataMappingService dataMappingService,
        IDateTimeService dateTimeService,
        ViewModelCollectionsManager collectionsManager,
        ILogger<IncidentCreateViewModel> logger)
    {
        _workflowService = workflowService;
        _orchestrationService = orchestrationService;
        _validationService = validationService;
        _dataMappingService = dataMappingService;
        _dateTimeService = dateTimeService;
        _collectionsManager = collectionsManager;
        _logger = logger;
        
        _logger.LogDebug("Initializing IncidentCreateViewModel - constructor only");
        
        // Initialize with current Eastern Time (business timezone)
        var easternTime = _dateTimeService.GetCurrentEasternTime();
        TimeIssueStarted = easternTime;
        TimeReported = easternTime;
        
        // ✅ IMPROVED: No async operations in constructor - explicit initialization required
    }

    /// <summary>
    /// Initializes ViewModel asynchronously.
    /// ✅ IMPROVED: Explicit async initialization following 2024 best practices.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            _logger.LogDebug("IncidentCreateViewModel already initialized, skipping");
            return;
        }

        try
        {
            _logger.LogDebug("Starting ViewModel async initialization");
            await _collectionsManager.LoadAllCollectionsAsync().ConfigureAwait(true);
            IsInitialized = true;
            _logger.LogInformation("IncidentCreateViewModel initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize IncidentCreateViewModel");
            ErrorMessage = "Failed to load form data. Please refresh the page.";
            IsInitialized = false;
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
    /// Selected impacted users count enum for ComboBox binding.
    /// ✅ IMPROVED: Single source of truth following 2024 best practices.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep), nameof(ImpactedUsers))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial ImpactedUsersCount? SelectedImpactedUsersCount { get; set; }
    
    /// <summary>
    /// Number of users impacted by the incident (computed from SelectedImpactedUsersCount).
    /// ✅ IMPROVED: Computed property eliminates synchronization issues.
    /// </summary>
    public int? ImpactedUsers => SelectedImpactedUsersCount switch
    {
        ImpactedUsersCount count => (int)count,
        null => null
    };

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
    /// Source from which the incident was reported.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial IncidentSource? IncidentSource { get; set; } = OperationPrime.Domain.Enums.IncidentSource.NOC;

    /// <summary>
    /// Indicates whether the incident is generating multiple calls/reports.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial GeneratingMultipleCalls? GeneratingMultipleCalls { get; set; } = OperationPrime.Domain.Enums.GeneratingMultipleCalls.No;

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident number is required")]
    [StringLength(ValidationLengths.IncidentNumberMaxLength, ErrorMessage = "Incident number cannot exceed {1} characters")]
    public partial string IncidentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Urgency level of the incident (1=High, 2=Medium, 3=Low).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UrgencyIndex), nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial int Urgency { get; set; } = UrgencyLevels.Default;

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
    public bool IsStep1 => CurrentStep == 1;
    public bool IsStep2 => CurrentStep == 2;
    public bool IsStep3 => CurrentStep == 3;
    public bool IsStep4 => CurrentStep == 4 && IsMajorIncident;
    public int TotalSteps => _workflowService.GetTotalSteps(IncidentType);
    public bool ShowNextButton => _workflowService.ShowNextButton(CurrentStep, IncidentType);
    public bool CanGoNext => _workflowService.CanGoNext(CurrentStep, IncidentType, IsCurrentStepValid());
    public bool CanGoPrevious => _workflowService.CanGoPrevious(CurrentStep);
    public bool IsLastStep => _workflowService.IsLastStep(CurrentStep, IncidentType);

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

    public int CurrentBreadcrumbIndex => CurrentStep - 1; // Convert 1-based step to 0-based index

    // ✅ CLEAN: Delegated Collection Properties (Managed by Helper)
    public ObservableCollection<IncidentType> AvailableIncidentTypes => _collectionsManager.IncidentTypes;
    public ObservableCollection<Priority> AvailablePriorities => _collectionsManager.Priorities;
    public ObservableCollection<Status> AvailableStatuses => _collectionsManager.Statuses;
    public ObservableCollection<ImpactedUsersCount> AvailableImpactedUsersCounts => _collectionsManager.ImpactedUsersCounts;
    public ObservableCollection<IncidentSource> AvailableIncidentSources => _collectionsManager.IncidentSources;
    public ObservableCollection<GeneratingMultipleCalls> AvailableGeneratingMultipleCallsOptions => _collectionsManager.GeneratingMultipleCallsOptions;
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

            var formData = _dataMappingService.MapFromViewModel(
                Title, Description, BusinessImpact, TimeIssueStarted, TimeReported,
                ImpactedUsers, ApplicationAffected, LocationsAffected, Workaround,
                IncidentNumber, Urgency, IncidentType, Priority, Status, SelectedImpactedUsersCount,
                IncidentSource ?? OperationPrime.Domain.Enums.IncidentSource.NOC,
                GeneratingMultipleCalls ?? OperationPrime.Domain.Enums.GeneratingMultipleCalls.No);
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
        // Validate step is within bounds for the incident type
        var totalSteps = _workflowService.GetTotalSteps(IncidentType);
        if (step >= 1 && step <= totalSteps)
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
        Urgency = UrgencyLevels.Default;
        IncidentSource = OperationPrime.Domain.Enums.IncidentSource.NOC;
        GeneratingMultipleCalls = OperationPrime.Domain.Enums.GeneratingMultipleCalls.No;
        CurrentStep = 1;
        
        // Clear all form fields
        Title = Description = BusinessImpact = string.Empty;
        ApplicationAffected = LocationsAffected = Workaround = IncidentNumber = string.Empty;
        
        // Reset dates to current Eastern Time
        var easternTime = _dateTimeService.GetCurrentEasternTime();
        TimeIssueStarted = TimeReported = easternTime;
        
        _logger.LogDebug("ViewModel Reset: TimeIssueStarted={TimeIssueStarted}, TimeReported={TimeReported}", 
            TimeIssueStarted, TimeReported);
        
        // Clear user count (only need to clear the source of truth)
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
        
        // ✅ CLEAN: Use consolidated validation service for comprehensive validation
        var formData = _dataMappingService.MapFromViewModel(
            Title, Description, BusinessImpact, TimeIssueStarted, TimeReported,
            ImpactedUsers, ApplicationAffected, LocationsAffected, Workaround,
            IncidentNumber, Urgency, IncidentType, Priority, Status, SelectedImpactedUsersCount,
            IncidentSource ?? OperationPrime.Domain.Enums.IncidentSource.NOC,
            GeneratingMultipleCalls ?? OperationPrime.Domain.Enums.GeneratingMultipleCalls.No);
        var validationResult = _validationService.ValidateCompleteIncidentData(formData);
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
    /// ✅ IMPROVED: Simplified validation delegation following 2024 best practices.
    /// </summary>
    private bool IsCurrentStepValid()
    {
        _logger.LogDebug("Validating step {CurrentStep} for {IncidentType}", CurrentStep, IncidentType);
        
        var formData = _dataMappingService.MapFromViewModel(
            Title, Description, BusinessImpact, TimeIssueStarted, TimeReported,
            ImpactedUsers, ApplicationAffected, LocationsAffected, Workaround,
            IncidentNumber, Urgency, IncidentType, Priority, Status, SelectedImpactedUsersCount,
            IncidentSource ?? OperationPrime.Domain.Enums.IncidentSource.NOC,
            GeneratingMultipleCalls ?? OperationPrime.Domain.Enums.GeneratingMultipleCalls.No);
        return _validationService.ValidateCurrentStep(CurrentStep, formData);
    }

}