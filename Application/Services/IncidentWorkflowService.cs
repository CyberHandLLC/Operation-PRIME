using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for incident workflow logic.
/// Extracted from IncidentCreateViewModel following Microsoft's 2024 MVVM Community Toolkit patterns.
/// </summary>
public class IncidentWorkflowService : IIncidentWorkflowService
{
    /// <summary>
    /// Determines the total number of steps for the given incident type
    /// </summary>
    public int GetTotalSteps(IncidentType incidentType)
    {
        return incidentType switch
        {
            IncidentType.PreIncident => 3,    // Pre-Incident: Type, Impact, Details
            IncidentType.MajorIncident => 4,  // Major Incident: Type, Impact, Details, Master Checklist
            _ => 3
        };
    }

    /// <summary>
    /// Determines if the current step is the last step for the given incident type
    /// </summary>
    public bool IsLastStep(int currentStep, IncidentType incidentType)
    {
        return currentStep >= GetTotalSteps(incidentType);
    }

    /// <summary>
    /// Determines if navigation to the previous step is allowed
    /// </summary>
    public bool CanGoPrevious(int currentStep)
    {
        return currentStep > 1;
    }

    /// <summary>
    /// Determines if navigation to the next step is allowed
    /// </summary>
    public bool CanGoNext(int currentStep, IncidentType incidentType, bool isStepValid)
    {
        // Cannot go next if current step is invalid
        if (!isStepValid)
            return false;

        // Cannot go next if already at the last step
        return currentStep < GetTotalSteps(incidentType);
    }

    /// <summary>
    /// Gets the next step number
    /// </summary>
    public int GetNextStep(int currentStep, IncidentType incidentType)
    {
        var totalSteps = GetTotalSteps(incidentType);
        return currentStep < totalSteps ? currentStep + 1 : currentStep;
    }

    /// <summary>
    /// Gets the previous step number
    /// </summary>
    public int GetPreviousStep(int currentStep)
    {
        return currentStep > 1 ? currentStep - 1 : currentStep;
    }

    /// <summary>
    /// Determines if the Next button should be visible
    /// </summary>
    public bool ShowNextButton(int currentStep, IncidentType incidentType)
    {
        return !IsLastStep(currentStep, incidentType);
    }

    /// <summary>
    /// Determines if the Submit button should be visible
    /// </summary>
    public bool ShowSubmitButton(int currentStep, IncidentType incidentType)
    {
        return IsLastStep(currentStep, incidentType);
    }
}