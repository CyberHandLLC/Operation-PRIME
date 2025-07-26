using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel orchestrating the multi-step incident creation wizard.
/// </summary>
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
