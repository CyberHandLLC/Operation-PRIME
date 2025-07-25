using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Workflow operations for major incidents including NOI generation.
/// </summary>
public interface IMajorIncidentWorkflowService
{
    Task<MajorIncident> StartAsync(MajorIncident incident);
    Task<MajorIncident> UpdateStatusAsync(MajorIncident incident, string statusUpdate, string updatedBy);
    Task<string> GenerateNoiAsync(MajorIncident incident, string templateType);
}
