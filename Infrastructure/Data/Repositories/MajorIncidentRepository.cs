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

    public MajorIncidentRepository(OperationPrimeDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MajorIncident entity)
    {
        _context.Set<MajorIncident>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _context.Set<MajorIncident>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MajorIncident>> GetAllAsync()
    {
        return await _context.Set<MajorIncident>().ToListAsync();
    }

    public async Task<MajorIncident?> GetAsync(Guid id)
    {
        return await _context.Set<MajorIncident>().FindAsync(id);
    }

    public async Task<IEnumerable<MajorIncident>> GetOpenAsync()
    {
        return await _context.Set<MajorIncident>()
            .Where(m => m.Status != Domain.Enums.IncidentStatus.Closed)
            .ToListAsync();
    }

    public async Task UpdateAsync(MajorIncident entity)
    {
        _context.Set<MajorIncident>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
