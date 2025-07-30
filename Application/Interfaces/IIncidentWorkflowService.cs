using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for incident workflow logic.
/// Follows Microsoft's 2024 MVVM Community Toolkit service extraction patterns.
/// </summary>
public interface IIncidentWorkflowService
{
    /// <summary>
    /// Determines the total number of steps for the given incident type
    /// </summary>
    /// <param name="incidentType">The incident type</param>
    /// <returns>Total number of steps (3 for Pre-Incident, 4 for Major Incident)</returns>
    int GetTotalSteps(IncidentType incidentType);

    /// <summary>
    /// Determines if the current step is the last step for the given incident type
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <param name="incidentType">The incident type</param>
    /// <returns>True if this is the last step</returns>
    bool IsLastStep(int currentStep, IncidentType incidentType);

    /// <summary>
    /// Determines if navigation to the previous step is allowed
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <returns>True if previous navigation is allowed</returns>
    bool CanGoPrevious(int currentStep);

    /// <summary>
    /// Determines if navigation to the next step is allowed
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <param name="incidentType">The incident type</param>
    /// <param name="isStepValid">Whether the current step validation passes</param>
    /// <returns>True if next navigation is allowed</returns>
    bool CanGoNext(int currentStep, IncidentType incidentType, bool isStepValid);

    /// <summary>
    /// Gets the next step number
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <param name="incidentType">The incident type</param>
    /// <returns>Next step number, or current step if at the end</returns>
    int GetNextStep(int currentStep, IncidentType incidentType);

    /// <summary>
    /// Gets the previous step number
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <returns>Previous step number, or current step if at the beginning</returns>
    int GetPreviousStep(int currentStep);

    /// <summary>
    /// Determines if the Next button should be visible
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <param name="incidentType">The incident type</param>
    /// <returns>True if Next button should be visible</returns>
    bool ShowNextButton(int currentStep, IncidentType incidentType);

    /// <summary>
    /// Determines if the Submit button should be visible
    /// </summary>
    /// <param name="currentStep">Current step number</param>
    /// <param name="incidentType">The incident type</param>
    /// <returns>True if Submit button should be visible</returns>
    bool ShowSubmitButton(int currentStep, IncidentType incidentType);
}