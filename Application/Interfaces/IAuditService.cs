using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Provides audit logging capabilities for incidents.
/// </summary>
public interface IAuditService
{
    Task LogChangeAsync(Guid incidentId, string field, string? oldValue, string? newValue, string user);
    Task LogActionAsync(Guid incidentId, string action, string description, string user);
    Task<IEnumerable<AuditLogEntry>> GetAuditTrailAsync(Guid incidentId);
    Task ExportAuditLogAsync(Guid incidentId, string filePath);
}
