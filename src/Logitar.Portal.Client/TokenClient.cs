using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Client;

internal class TokenClient : ClientBase, ITokenService
{
  private const string Path = "/api/tokens";

  public TokenClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<CreatedToken>($"{Path}/create", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<ValidatedToken>($"{Path}/validate", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }
}
