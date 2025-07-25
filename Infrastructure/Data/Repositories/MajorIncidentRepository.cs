using Microsoft.EntityFrameworkCore;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Interfaces;

namespace OperationPrime.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for major incidents using EF Core.
/// </summary>
public class MajorIncidentRepository : IMajorIncidentRepository
{
    private readonly OperationPrimeDbContext _context;
    private readonly ILogger<MajorIncidentRepository> _logger;

    public MajorIncidentRepository(OperationPrimeDbContext context, ILogger<MajorIncidentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(MajorIncident entity)
    {
        _logger.LogDebug("Adding major incident {Id}", entity.Id);
        _context.Set<MajorIncident>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug("Deleting major incident {Id}", id);
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _context.Set<MajorIncident>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MajorIncident>> GetAllAsync()
    {
        _logger.LogDebug("Retrieving all major incidents");
        return await _context.Set<MajorIncident>().ToListAsync();
    }

    public async Task<MajorIncident?> GetAsync(Guid id)
    {
        _logger.LogDebug("Retrieving major incident {Id}", id);
        return await _context.Set<MajorIncident>().FindAsync(id);
    }

    public async Task<IEnumerable<MajorIncident>> GetOpenAsync()
    {
        _logger.LogDebug("Retrieving open major incidents");
        return await _context.Set<MajorIncident>()
            .Where(m => m.Status != Domain.Enums.IncidentStatus.Closed)
            .ToListAsync();
    }

    public async Task UpdateAsync(MajorIncident entity)
    {
        _logger.LogDebug("Updating major incident {Id}", entity.Id);
        _context.Set<MajorIncident>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
