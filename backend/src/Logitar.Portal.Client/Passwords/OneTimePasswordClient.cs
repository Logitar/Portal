using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Client.Passwords;

internal class OneTimePasswordClient : BaseClient, IOneTimePasswordClient
{
  private const string Path = "/api/one-time-passwords";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public OneTimePasswordClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, IRequestContext? context)
  {
    return await PostAsync<OneTimePassword>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<OneTimePassword?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<OneTimePassword>(uri, context);
  }

  public async Task<OneTimePassword?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<OneTimePassword>(uri, context);
  }

  public async Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<OneTimePassword>(uri, payload, context);
  }
}
