using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Services;

/// <summary>
/// Simple NOI generation using predefined templates.
/// </summary>
public class NOIService : INOIService
{
    private readonly ILogger<NOIService> _logger;

    public NOIService(ILogger<NOIService> logger)
    {
        _logger = logger;
    }

    public string GenerateNOI(Incident incident, string templateType)
    {
        _logger.LogDebug("Generating NOI using template {Template}", templateType);
        // Placeholder: real template rendering to be implemented
        return $"NOI for {incident.Title} ({templateType})";
    }

    public Task<bool> SendNOIAsync(string content, string[] recipients)
    {
        _logger.LogInformation("Sending NOI to {Count} recipients", recipients.Length);
        // Placeholder: integrate with email service
        return Task.FromResult(true);
    }
}
