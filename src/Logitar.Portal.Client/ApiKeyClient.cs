using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Client;

internal class ApiKeyClient : ClientBase, IApiKeyService
{
  private const string Path = "/api/keys";

  public ApiKeyClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<ApiKey> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken)
  {
    return await PatchAsync<ApiKey>($"{Path}/authenticate/{xApiKey}", cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<ApiKey>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<ApiKey?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<ApiKey>($"{Path}/{id}", cancellationToken);
  }

  public async Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync<ApiKey>($"{Path}/{id}", cancellationToken);
  }

  public async Task<ApiKey?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<ApiKey>(path.ToString(), payload, cancellationToken);
  }

  public async Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<ApiKey>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<ApiKey>($"{Path}/{id}", payload, cancellationToken);
  }
}
