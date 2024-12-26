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

  public async Task<OneTimePasswordModel> CreateAsync(CreateOneTimePasswordPayload payload, IRequestContext? context)
  {
    return await PostAsync<OneTimePasswordModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<OneTimePasswordModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<OneTimePasswordModel>(uri, context);
  }

  public async Task<OneTimePasswordModel?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<OneTimePasswordModel>(uri, context);
  }

  public async Task<OneTimePasswordModel?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<OneTimePasswordModel>(uri, payload, context);
  }
}
