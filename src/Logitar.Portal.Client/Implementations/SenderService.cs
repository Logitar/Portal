using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Client.Implementations;

internal class SenderService : HttpService, ISenderService
{
  private const string BasePath = "/senders";

  public SenderService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Sender> CreateAsync(CreateSenderInput input, CancellationToken cancellationToken)
    => await PostAsync<Sender>(BasePath, input, cancellationToken);

  public async Task<Sender> DeleteAsync(Guid id, CancellationToken cancellationToken)
    => await DeleteAsync<Sender>($"{BasePath}/{id}", cancellationToken);

  public async Task<Sender?> GetAsync(Guid? id, string? defaultInRealm, CancellationToken cancellationToken)
  {
    if (id.HasValue)
    {
      if (defaultInRealm != null)
      {
        throw new NotSupportedException($"You may only specify one of the following parameters: '{nameof(id)}', '{nameof(defaultInRealm)}'.");
      }

      return await GetAsync<Sender>($"{BasePath}/{id.Value}", cancellationToken);
    }
    else if (defaultInRealm != null)
    {
      return await GetAsync<Sender>($"{BasePath}/default/{defaultInRealm}", cancellationToken);
    }

    return null;
  }

  public async Task<PagedList<Sender>> GetAsync(ProviderType? provider, string? realm, string? search,
    SenderSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(provider)] = provider,
      [nameof(realm)] = realm,
      [nameof(search)] = search,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<Sender>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<Sender> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<Sender>($"{BasePath}/{id}/default", cancellationToken);

  public async Task<Sender> UpdateAsync(Guid id, UpdateSenderInput input, CancellationToken cancellationToken)
    => await PutAsync<Sender>($"{BasePath}/{id}", input, cancellationToken);
}
