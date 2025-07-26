using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;

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
    }
}
