using OperationPrime.Domain.Enums;

namespace OperationPrime.Presentation.Helpers;

/// <summary>
/// Helper class for step navigation logic to reduce ViewModel complexity.
/// Follows Single Responsibility Principle for step-related computations.
/// </summary>
public static class StepNavigationHelper
{
    /// <summary>
    /// Gets the total number of steps for an incident type.
    /// </summary>
    public static int GetTotalSteps(IncidentType incidentType) => incidentType switch
    {
        IncidentType.MajorIncident => 4,
        _ => 3
    };

    /// <summary>
    /// Determines if the current step is a specific step number.
    /// </summary>
    public static bool IsStep(int currentStep, int targetStep) => currentStep == targetStep;

    /// <summary>
    /// Determines if the current step is the last step.
    /// </summary>
    public static bool IsLastStep(int currentStep, IncidentType incidentType) => 
        currentStep >= GetTotalSteps(incidentType);

    /// <summary>
    /// Determines if the Next button should be shown.
    /// </summary>
    public static bool ShouldShowNextButton(int currentStep, IncidentType incidentType) => 
        !IsLastStep(currentStep, incidentType);

    /// <summary>
    /// Gets the breadcrumb index (0-based).
    /// </summary>
    public static int GetBreadcrumbIndex(int currentStep) => currentStep - 1;

    /// <summary>
    /// Validates if a step number is valid for the incident type.
    /// </summary>
    public static bool IsValidStep(int step, IncidentType incidentType) => 
        step >= 1 && step <= GetTotalSteps(incidentType);
} 