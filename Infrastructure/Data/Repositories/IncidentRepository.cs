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

    public IncidentRepository(OperationPrimeDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Incident entity)
    {
        _context.Incidents.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _context.Incidents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        return await _context.Incidents.ToListAsync();
    }

    public async Task<Incident?> GetAsync(Guid id)
    {
        return await _context.Incidents.FindAsync(id);
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(Domain.Enums.IncidentStatus status)
    {
        return await _context.Incidents.Where(i => i.Status == status).ToListAsync();
    }

    public async Task UpdateAsync(Incident entity)
    {
        _context.Incidents.Update(entity);
        await _context.SaveChangesAsync();
    }
}
