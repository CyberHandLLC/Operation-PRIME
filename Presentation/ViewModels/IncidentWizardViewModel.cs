using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel orchestrating the multi-step incident creation wizard.
/// </summary>
public partial class IncidentWizardViewModel : WizardViewModelBase<IncidentWizardViewModel>
{
    private readonly IPreIncidentWorkflowService _preWorkflow;
    private readonly IMajorIncidentWorkflowService _majorWorkflow;

    public PreIncidentViewModel PreIncident { get; }
    public MajorIncidentViewModel MajorIncident { get; }

    public IncidentType IncidentType { get; private set; } = IncidentType.PreIncident;

    public bool IsMajor => IncidentType == IncidentType.MajorIncident;

    public IncidentWizardViewModel(IPreIncidentWorkflowService preWorkflow,
        IMajorIncidentWorkflowService majorWorkflow,
        PreIncidentViewModel preIncident,
        MajorIncidentViewModel majorIncident,
        ILogger<IncidentWizardViewModel> logger)
        : base(logger)
    {
        _preWorkflow = preWorkflow;
        _majorWorkflow = majorWorkflow;
        PreIncident = preIncident;
        MajorIncident = majorIncident;
    }

    public void Initialize(IncidentType incidentType)
    {
        IncidentType = incidentType;
        Logger.LogDebug("Incident wizard initialized for {Type}", incidentType);
    public IncidentWizardViewModel(ILogger<IncidentWizardViewModel> logger)
        : base(logger)
    {
public partial class IncidentWizardViewModel : BaseViewModel
{
    private readonly ILogger<IncidentWizardViewModel> _logger;

    public IncidentWizardViewModel(ILogger<IncidentWizardViewModel> logger)
    {
        _logger = logger;
        _logger.LogDebug("IncidentWizardViewModel initialized");
    }

    /// <summary>
    /// Gets or sets the current wizard step index.
    /// </summary>
    [ObservableProperty]
    private int stepIndex;

    /// <summary>
    /// Advances to the next wizard step.
    /// </summary>
    [RelayCommand]
    private void NextStep()
    {
        StepIndex++;
        _logger.LogDebug("Advanced to step {Step}", StepIndex);
    }

    /// <summary>
    /// Returns to the previous wizard step if possible.
    /// </summary>
    [RelayCommand]
    private void PreviousStep()
    {
        if (StepIndex > 0)
        {
            StepIndex--;
            _logger.LogDebug("Returned to step {Step}", StepIndex);
        }
    }
}
