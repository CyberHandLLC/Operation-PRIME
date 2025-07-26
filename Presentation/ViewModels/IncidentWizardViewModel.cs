using Microsoft.Extensions.Logging;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel orchestrating the multi-step incident creation wizard.
/// </summary>
public partial class IncidentWizardViewModel : WizardViewModelBase<IncidentWizardViewModel>
{
    public IncidentWizardViewModel(ILogger<IncidentWizardViewModel> logger)
        : base(logger)
    {
    }
}
