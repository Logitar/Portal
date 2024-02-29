using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Client.Tokens;

internal class TokenClient : BaseClient, ITokenClient
{
  private const string Path = "/api/tokens";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public TokenClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, IRequestContext? context)
  {
    return await PostAsync<CreatedToken>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, IRequestContext? context)
  {
    return await PutAsync<ValidatedToken>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(ValidateAsync), HttpMethod.Put, UriPath, payload, context);
  }
}
