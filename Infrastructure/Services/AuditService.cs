using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Infrastructure.Data;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Persists audit log entries to the database and provides retrieval helpers.
/// </summary>
public class AuditService : IAuditService
{
    private readonly OperationPrimeDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(OperationPrimeDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogChangeAsync(Guid incidentId, string field, string? oldValue, string? newValue, string user)
    {
        var entry = new AuditLogEntry
        {
            IncidentId = incidentId,
            Field = field,
            OldValue = oldValue,
            NewValue = newValue,
            Action = "Change",
            User = user,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogEntries.Add(entry);
        await _context.SaveChangesAsync();
        _logger.LogDebug("Audit change logged for incident {IncidentId}: {Field}", incidentId, field);
    }

    public async Task LogActionAsync(Guid incidentId, string action, string description, string user)
    {
        var entry = new AuditLogEntry
        {
            IncidentId = incidentId,
            Action = action,
            NewValue = description,
            User = user,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogEntries.Add(entry);
        await _context.SaveChangesAsync();
        _logger.LogDebug("Audit action {Action} logged for incident {IncidentId}", action, incidentId);
    }

    public async Task<IEnumerable<AuditLogEntry>> GetAuditTrailAsync(Guid incidentId)
    {
        return await _context.AuditLogEntries
            .Where(a => a.IncidentId == incidentId)
            .OrderBy(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task ExportAuditLogAsync(Guid incidentId, string filePath)
    {
        var entries = await GetAuditTrailAsync(incidentId);
        var lines = entries.Select(e => $"{e.Timestamp:o}\t{e.User}\t{e.Action}\t{e.Field}\t{e.OldValue}\t{e.NewValue}");
        await File.WriteAllLinesAsync(filePath, lines);
        _logger.LogInformation("Audit log exported for incident {IncidentId} to {Path}", incidentId, filePath);
    }
}
