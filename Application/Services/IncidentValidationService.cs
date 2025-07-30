using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for incident validation logic.
/// Extracted from IncidentCreateViewModel following Microsoft's 2024 MVVM Community Toolkit patterns.
/// </summary>
public class IncidentValidationService : IIncidentValidationService
{
    /// <summary>
    /// Validates Step 1: Incident Type Selection
    /// </summary>
    public bool ValidateStep1(IncidentType incidentType)
    {
        // Step 1 is always valid as IncidentType has a default value
        return true;
    }

    /// <summary>
    /// Validates Step 2: Impact Assessment
    /// </summary>
    public bool ValidateStep2(int? impactedUsers, int urgency, string? businessImpact, 
        string? applicationAffected, string? locationsAffected, string? workaround)
    {
        // Step 2 fields required for Pre-Incident workflow
        // Note: businessImpact and workaround are NOT required in Step 2
        // Workaround is optional (marked as "Workaround (Optional)" in UI)
        return impactedUsers.HasValue &&
               urgency >= 1 && urgency <= 5 &&
               !string.IsNullOrWhiteSpace(applicationAffected) &&
               !string.IsNullOrWhiteSpace(locationsAffected);
    }

    /// <summary>
    /// Validates Step 3: Incident Details
    /// </summary>
    public bool ValidateStep3(string? incidentNumber, string? title, string? description, 
        DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported)
    {
        // Core incident details are required
        return !string.IsNullOrWhiteSpace(incidentNumber) &&
               !string.IsNullOrWhiteSpace(title) &&
               !string.IsNullOrWhiteSpace(description) &&
               timeIssueStarted.HasValue &&
               timeReported.HasValue;
    }

    /// <summary>
    /// Validates Step 4: Master Checklist (Major Incidents only)
    /// </summary>
    public bool ValidateStep4(IncidentType incidentType, Priority priority, Status status)
    {
        // Step 4 is only required for Major Incidents
        if (incidentType == IncidentType.PreIncident)
        {
            return true; // Pre-incidents don't have Step 4
        }

        // Major incidents require valid priority and status
        return Enum.IsDefined(typeof(Priority), priority) &&
               Enum.IsDefined(typeof(Status), status);
    }

    /// <summary>
    /// Validates all required properties for incident creation based on incident type
    /// </summary>
    public bool ValidateAllProperties(IncidentType incidentType, string? incidentNumber, string? title, string? description,
        int? impactedUsers, int urgency, string? businessImpact, string? applicationAffected,
        string? locationsAffected, string? workaround, DateTimeOffset? timeIssueStarted,
        DateTimeOffset? timeReported)
    {
        // Validate core required fields (required for all incident types)
        if (string.IsNullOrWhiteSpace(incidentNumber) ||
            string.IsNullOrWhiteSpace(title) ||
            string.IsNullOrWhiteSpace(description) ||
            !timeIssueStarted.HasValue ||
            !timeReported.HasValue)
        {
            return false;
        }

        // Validate basic impact assessment fields (required for all incident types)
        if (!impactedUsers.HasValue ||
            urgency < 1 || urgency > 5 ||
            string.IsNullOrWhiteSpace(applicationAffected) ||
            string.IsNullOrWhiteSpace(locationsAffected))
        {
            return false;
        }

        // Major Incident specific requirements
        if (incidentType == IncidentType.MajorIncident)
        {
            // Major incidents require business impact (workaround is optional for all incident types)
            if (string.IsNullOrWhiteSpace(businessImpact))
            {
                return false;
            }
        }
        // Pre-Incident specific requirements
        // Note: Pre-incidents don't require any additional fields beyond the basic ones
        // Workaround is optional for all incident types (marked as "Workaround (Optional)" in UI)

        // Validate string lengths (matching original validation attributes)
        if (title.Length > 200 ||
            description.Length > 2000 ||
            applicationAffected.Length > 100 ||
            locationsAffected.Length > 500)
        {
            return false;
        }

        // Validate optional field lengths if they exist
        if (!string.IsNullOrWhiteSpace(businessImpact) && businessImpact.Length > 1000)
        {
            return false;
        }
        
        if (!string.IsNullOrWhiteSpace(workaround) && workaround.Length > 1000)
        {
            return false;
        }

        return true;
    }
}