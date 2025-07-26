using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Provides Notice of Incident (NOI) generation capabilities.
/// </summary>
public interface INOIService
{
    /// <summary>
    /// Generates NOI content based on the incident and template type.
    /// </summary>
    /// <param name="incident">Incident data for the NOI.</param>
    /// <param name="templateType">Template to apply.</param>
    /// <returns>Formatted NOI content.</returns>
    string GenerateNOI(Incident incident, string templateType);

    /// <summary>
    /// Sends NOI content to the provided recipients.
    /// </summary>
    /// <param name="content">NOI body.</param>
    /// <param name="recipients">Email recipients.</param>
    /// <returns>True if send succeeded.</returns>
    Task<bool> SendNOIAsync(string content, string[] recipients);
}
