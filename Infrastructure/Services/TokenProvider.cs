using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Simple HTTP-based token provider.
/// </summary>
public class TokenProvider : ITokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider(HttpClient httpClient, ILogger<TokenProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<TokenResponse>("token");
            return response?.Token ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch token");
            return string.Empty;
        }
    }

    private sealed record TokenResponse(string Token);
}
