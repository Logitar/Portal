using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client;

internal class ConfigurationClient : IConfigurationService
{
  private readonly HttpClient _client;

  public ConfigurationClient(HttpClient client, PortalSettings settings)
  {
    _client = client;
    _client.BaseAddress = new Uri(settings.BaseUrl);
  }

  public async Task<Session> InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    Uri requestUri = new("/api/configuration", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Post, requestUri)
    {
      Content = JsonContent.Create(payload)
    };

    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    return new Session();
  }

  public Task<Configuration?> ReadAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
