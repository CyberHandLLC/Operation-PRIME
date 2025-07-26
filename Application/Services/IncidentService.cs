using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Services;

/// <summary>
/// Simple in-memory implementation of incident service for development.
/// </summary>
public class IncidentService : IIncidentService
{
    private static readonly List<Incident> _incidents = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Gets all incidents in the system.
    /// </summary>
    /// <returns>A list of all incidents.</returns>
    public Task<List<Incident>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new List<Incident>(_incidents));
        }
    }

    /// <summary>
    /// Gets an incident by its unique identifier.
    /// </summary>
    /// <param name="id">The incident ID.</param>
    /// <returns>The incident if found, null otherwise.</returns>
    public Task<Incident?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(incident);
        }
    }

    /// <summary>
    /// Creates a new incident.
    /// </summary>
    /// <param name="incident">The incident to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateAsync(Incident incident)
    {
        if (incident == null)
            throw new ArgumentNullException(nameof(incident));

        lock (_lock)
        {
            incident.CreatedAt = DateTime.UtcNow;
            incident.UpdatedAt = DateTime.UtcNow;
            _incidents.Add(incident);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing incident.
    /// </summary>
    /// <param name="incident">The incident to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateAsync(Incident incident)
    {
        if (incident == null)
            throw new ArgumentNullException(nameof(incident));

        lock (_lock)
        {
            var existingIncident = _incidents.FirstOrDefault(i => i.Id == incident.Id);
            if (existingIncident != null)
            {
                var index = _incidents.IndexOf(existingIncident);
                incident.UpdatedAt = DateTime.UtcNow;
                _incidents[index] = incident;
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes an incident by its unique identifier.
    /// </summary>
    /// <param name="id">The incident ID to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync(Guid id)
    {
        lock (_lock)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);
            if (incident != null)
            {
                _incidents.Remove(incident);
            }
        }

        return Task.CompletedTask;
    }
}
