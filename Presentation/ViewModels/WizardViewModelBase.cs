using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// Base class for wizard style ViewModels with step tracking logic.
/// Provides Next/Previous commands and exposes the current step index.
/// </summary>
public abstract partial class WizardViewModelBase<TViewModel> : BaseViewModel
    where TViewModel : class
{
    protected ILogger<TViewModel> Logger { get; }

    protected WizardViewModelBase(ILogger<TViewModel> logger)
    {
        Logger = logger;
        Logger.LogDebug("{ViewModel} initialized", typeof(TViewModel).Name);
    private readonly ILogger<TViewModel> _logger;

    protected WizardViewModelBase(ILogger<TViewModel> logger)
    {
        _logger = logger;
        _logger.LogDebug("{ViewModel} initialized", typeof(TViewModel).Name);
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
    public void NextStep()
    {
        StepIndex++;
        Logger.LogDebug("Advanced to step {Step}", StepIndex);
        _logger.LogDebug("Advanced to step {Step}", StepIndex);
    }

    /// <summary>
    /// Returns to the previous wizard step if possible.
    /// </summary>
    [RelayCommand]
    public void PreviousStep()
    {
        if (StepIndex > 0)
        {
            StepIndex--;
            Logger.LogDebug("Returned to step {Step}", StepIndex);
            _logger.LogDebug("Returned to step {Step}", StepIndex);
        }
    }
}
