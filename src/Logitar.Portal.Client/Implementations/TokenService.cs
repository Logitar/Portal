using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Client.Implementations;

internal class TokenService : HttpService, ITokenService
{
  private const string BasePath = "/tokens";

  public TokenService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<ValidatedToken> ConsumeAsync(ValidateTokenInput input, CancellationToken cancellationToken = default)
    => await PostAsync<ValidatedToken>($"{BasePath}/consume", input, cancellationToken);

  public async Task<CreatedToken> CreateAsync(CreateTokenInput input, CancellationToken cancellationToken)
    => await PostAsync<CreatedToken>($"{BasePath}/create", input, cancellationToken);

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenInput input, CancellationToken cancellationToken)
    => await PostAsync<ValidatedToken>($"{BasePath}/validate", input, cancellationToken);
}
