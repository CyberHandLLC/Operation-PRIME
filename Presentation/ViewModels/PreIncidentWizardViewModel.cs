using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// Wizard ViewModel for creating pre-incidents.
/// Contains pre-incident specific step validation logic.
/// </summary>
public partial class PreIncidentWizardViewModel : WizardViewModelBase<PreIncidentWizardViewModel>
{
    private readonly IPreIncidentWorkflowService _workflow;

    public ObservableCollection<string> PreIncidentSources { get; } = new()
    {
        "Service Desk", "NOC", "SME"
    };

    public PreIncidentWizardViewModel(IPreIncidentWorkflowService workflow, ILogger<PreIncidentWizardViewModel> logger)
        : base(logger)
    {
        _workflow = workflow;
    }
}
