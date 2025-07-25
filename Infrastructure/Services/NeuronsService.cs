using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Provides access to Neurons integration using HttpClient.
/// </summary>
public class NeuronsService : INeuronsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NeuronsService> _logger;

    public NeuronsService(HttpClient httpClient, ILogger<NeuronsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Incident?> FetchIncidentAsync(string incidentNumber)
    {
        _logger.LogDebug("Fetching incident {IncidentNumber} from Neurons", incidentNumber);
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Incident>($"incidents/{incidentNumber}");
            _logger.LogDebug("Received incident data for {IncidentNumber}", incidentNumber);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching incident {IncidentNumber}", incidentNumber);
            return null;
        }
    }
}
