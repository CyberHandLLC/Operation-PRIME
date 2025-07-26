using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Workflow operations for creating and managing pre-incidents.
/// </summary>
public interface IPreIncidentWorkflowService
{
    Task<PreIncident> StartAsync(PreIncident incident);
    Task<PreIncident> CompleteStepAsync(PreIncident incident, int stepIndex);
    Task<MajorIncident> EscalateAsync(PreIncident incident, MajorIncident majorIncident);
}
