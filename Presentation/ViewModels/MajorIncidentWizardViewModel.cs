using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// Wizard ViewModel for creating major incidents with NOI requirements.
/// </summary>
public partial class MajorIncidentWizardViewModel : WizardViewModelBase<MajorIncidentWizardViewModel>
{
    private readonly IMajorIncidentWorkflowService _workflow;

    public ObservableCollection<string> EscalationContacts { get; } = new();

    public bool RequiresNOI => true;

    public MajorIncidentWizardViewModel(IMajorIncidentWorkflowService workflow, ILogger<MajorIncidentWizardViewModel> logger)
        : base(logger)
    {
        _workflow = workflow;
    }
}
