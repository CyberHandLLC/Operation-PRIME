using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Interfaces;

namespace OperationPrime.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for pre-incidents using EF Core.
/// </summary>
public class PreIncidentRepository : IPreIncidentRepository
{
    private readonly OperationPrimeDbContext _context;
    private readonly ILogger<PreIncidentRepository> _logger;

    public PreIncidentRepository(OperationPrimeDbContext context, ILogger<PreIncidentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(PreIncident entity)
    {
        _logger.LogDebug("Adding pre-incident {Id}", entity.Id);
        _context.Set<PreIncident>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug("Deleting pre-incident {Id}", id);
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _context.Set<PreIncident>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PreIncident>> GetAllAsync()
    {
        _logger.LogDebug("Retrieving all pre-incidents");
        return await _context.Set<PreIncident>().ToListAsync();
    }

    public async Task<PreIncident?> GetAsync(Guid id)
    {
        _logger.LogDebug("Retrieving pre-incident {Id}", id);
        return await _context.Set<PreIncident>().FindAsync(id);
    }

    public async Task<IEnumerable<PreIncident>> GetEscalatedAsync()
    {
        _logger.LogDebug("Retrieving escalated pre-incidents");
        return await _context.Set<PreIncident>()
            .Where(p => p.IsEscalated)
            .ToListAsync();
    }

    public async Task UpdateAsync(PreIncident entity)
    {
        _logger.LogDebug("Updating pre-incident {Id}", entity.Id);
        _context.Set<PreIncident>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
