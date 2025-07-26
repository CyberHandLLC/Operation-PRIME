using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

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
    }

    /// <summary>
    /// Gets or sets the current wizard step index.
    /// </summary>
    [ObservableProperty]
    private int stepIndex;

    /// <summary>
    /// Performs validation for the specified step.
    /// </summary>
    /// <param name="stepIndex">The step index to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating the result.</returns>
    protected virtual ValidationResult ValidateStep(int stepIndex) => ValidationResult.Success!;

    /// <summary>
    /// Advances to the next wizard step.
    /// </summary>
    [RelayCommand]
    public void NextStep()
    {
        var result = ValidateStep(StepIndex);
        if (result == ValidationResult.Success)
        {
            StepIndex++;
            Logger.LogDebug("Advanced to step {Step}", StepIndex);
        }
        else
        {
            Logger.LogWarning("Step {Step} validation failed: {Message}", StepIndex, result.ErrorMessage);
        }
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
        }
    }
}
