using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Client;

internal class DictionaryClient : ClientBase, IDictionaryService
{
  private const string Path = "/api/dictionaries";

  public DictionaryClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Dictionary>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<Dictionary>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Dictionary?> ReadAsync(Guid? id, string? realm, string? locale, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Dictionary> dictionaries = new(capacity: 2);

    if (id.HasValue)
    {
      Dictionary? dictionary = await GetAsync<Dictionary>($"{Path}/{id}", cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (locale != null)
    {
      StringBuilder path = new();

      path.Append(Path).Append("/locale:").Append(locale);
      if (realm != null)
      {
        path.Append("?realm=").Append(realm);
      }

      Dictionary? dictionary = await GetAsync<Dictionary>(path.ToString(), cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw new TooManyResultsException<Dictionary>(expected: 1, actual: dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }

  public async Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<Dictionary>(path.ToString(), payload, cancellationToken);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Dictionary>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<Dictionary>($"{Path}/{id}", payload, cancellationToken);
  }
}
