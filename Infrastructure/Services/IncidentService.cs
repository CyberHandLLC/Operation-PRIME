using Microsoft.EntityFrameworkCore;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Infrastructure.Data;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Service implementation for incident data operations.
/// Uses Entity Framework Core with SQLite for data persistence.
/// </summary>
public class IncidentService : IIncidentService
{
    private readonly OperationPrimeDbContext _context;

    /// <summary>
    /// Initializes a new instance of the IncidentService.
    /// </summary>
    /// <param name="context">Database context for data operations.</param>
    public IncidentService(OperationPrimeDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all incidents from the database.
    /// Ensures database is created before querying.
    /// </summary>
    /// <returns>A collection of all incidents.</returns>
    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        // Ensure database is created
        await _context.Database.EnsureCreatedAsync();

        // Return all incidents ordered by creation date (newest first)
        return await _context.Incidents
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();
    }


}
