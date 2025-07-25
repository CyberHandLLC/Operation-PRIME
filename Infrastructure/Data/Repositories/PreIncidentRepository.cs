using Microsoft.EntityFrameworkCore;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Interfaces;

namespace OperationPrime.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for pre-incidents using EF Core.
/// </summary>
public class PreIncidentRepository : IPreIncidentRepository
{
    private readonly OperationPrimeDbContext _context;

    public PreIncidentRepository(OperationPrimeDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PreIncident entity)
    {
        _context.Set<PreIncident>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _context.Set<PreIncident>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PreIncident>> GetAllAsync()
    {
        return await _context.Set<PreIncident>().ToListAsync();
    }

    public async Task<PreIncident?> GetAsync(Guid id)
    {
        return await _context.Set<PreIncident>().FindAsync(id);
    }

    public async Task<IEnumerable<PreIncident>> GetEscalatedAsync()
    {
        return await _context.Set<PreIncident>()
            .Where(p => p.IsEscalated)
            .ToListAsync();
    }

    public async Task UpdateAsync(PreIncident entity)
    {
        _context.Set<PreIncident>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
