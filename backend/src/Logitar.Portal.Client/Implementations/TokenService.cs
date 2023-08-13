using Logitar.Portal.Core.Tokens.Models;
using Logitar.Portal.Core.Tokens.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class TokenService : HttpService, ITokenService
  {
    private const string BasePath = "/tokens";

    public TokenService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<ValidatedTokenModel> ConsumeAsync(ValidateTokenPayload payload, CancellationToken cancellationToken = default)
      => await PostAsync<ValidatedTokenModel>($"{BasePath}/consume", payload, cancellationToken);

    public async Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default)
      => await PostAsync<TokenModel>($"{BasePath}/create", payload, cancellationToken);

    public async Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken = default)
      => await PostAsync<ValidatedTokenModel>($"{BasePath}/validate", payload, cancellationToken);
  }
}
