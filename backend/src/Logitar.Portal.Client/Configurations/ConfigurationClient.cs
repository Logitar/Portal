using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client.Configurations;

internal class ConfigurationClient : BaseClient, IConfigurationClient
{
  private const string Path = "/api/configuration";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public ConfigurationClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public Task<Session> InitializeAsync(InitializeConfigurationPayload payload, IRequestContext? context)
  {
    throw new NotSupportedException($"The operation '{nameof(ConfigurationClient)}.{nameof(InitializeAsync)}' is not supported.");
  }

  public async Task<Configuration?> ReadAsync(IRequestContext? context)
  {
    return await GetAsync<Configuration>(UriPath, context);
  }

  public async Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath(Path).SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<Configuration>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(ReplaceAsync), HttpMethod.Put, uri, payload, context);
  }

  public async Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, IRequestContext? context)
  {
    return await PatchAsync<Configuration>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(UpdateAsync), HttpMethod.Patch, UriPath, payload, context);
  }
}
