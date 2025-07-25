namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Provides authentication tokens for external API calls.
/// </summary>
public interface ITokenProvider
{
    Task<string> GetTokenAsync();
}
