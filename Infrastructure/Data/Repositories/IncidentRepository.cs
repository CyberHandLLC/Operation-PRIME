using Microsoft.EntityFrameworkCore;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Interfaces;

namespace OperationPrime.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for incidents using EF Core.
/// </summary>
public class IncidentRepository : IIncidentRepository
{
    private readonly OperationPrimeDbContext _context;
    private readonly ILogger<IncidentRepository> _logger;

    public IncidentRepository(OperationPrimeDbContext context, ILogger<IncidentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Incident entity)
    {
        _logger.LogDebug("Adding incident {Id}", entity.Id);
        _context.Incidents.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug("Deleting incident {Id}", id);
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _context.Incidents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        _logger.LogDebug("Retrieving all incidents from database");
        return await _context.Incidents.ToListAsync();
    }

    public async Task<Incident?> GetAsync(Guid id)
    {
        _logger.LogDebug("Retrieving incident {Id} from database", id);
        return await _context.Incidents.FindAsync(id);
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(Domain.Enums.IncidentStatus status)
    {
        _logger.LogDebug("Retrieving incidents by status {Status}", status);
        return await _context.Incidents.Where(i => i.Status == status).ToListAsync();
    }

    public async Task UpdateAsync(Incident entity)
    {
        _logger.LogDebug("Updating incident {Id}", entity.Id);
        _context.Incidents.Update(entity);
        await _context.SaveChangesAsync();
    }
}
