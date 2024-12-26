using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Client.Configurations;

internal class ConfigurationClient : BaseClient, IConfigurationClient
{
  private const string Path = "/api/configuration";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public ConfigurationClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<ConfigurationModel> ReadAsync(IRequestContext? context)
  {
    return await GetAsync<ConfigurationModel>(UriPath, context)
      ?? throw CreateInvalidApiResponseException(nameof(ReadAsync), HttpMethod.Get, UriPath, content: null, context);
  }

  public async Task<ConfigurationModel> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath(Path).SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<ConfigurationModel>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(ReplaceAsync), HttpMethod.Put, uri, payload, context);
  }

  public async Task<ConfigurationModel> UpdateAsync(UpdateConfigurationPayload payload, IRequestContext? context)
  {
    return await PatchAsync<ConfigurationModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(UpdateAsync), HttpMethod.Patch, UriPath, payload, context);
  }
}
