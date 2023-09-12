using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Client;

internal class SenderClient : ClientBase, ISenderService
{
  private const string Path = "/api/senders";

  public SenderClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Sender> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Sender>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<Sender?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<Sender>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Sender?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync<Sender>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Sender?> ReadDefaultAsync(string? realm, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append("/default");

    if (realm != null)
    {
      path.Append("?realm=").Append(realm);
    }

    return await GetAsync<Sender>(path.ToString(), cancellationToken);
  }

  public async Task<Sender?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<Sender>(path.ToString(), payload, cancellationToken);
  }

  public async Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Sender>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<Sender?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await PatchAsync<Sender>($"{Path}/{id}/default", cancellationToken);
  }

  public async Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<Sender>($"{Path}/{id}", payload, cancellationToken);
  }
}
