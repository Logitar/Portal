using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Client.Dictionaries;

internal class DictionaryClient : BaseClient, IDictionaryClient
{
  private const string Path = "/api/dictionaries";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public DictionaryClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, IRequestContext? context)
  {
    return await PostAsync<Dictionary>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<Dictionary?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<Dictionary>(uri, context);
  }

  public async Task<Dictionary?> ReadAsync(Guid? id, string? locale, IRequestContext? context)
  {
    Dictionary<Guid, Dictionary> dictionaries = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Dictionary? dictionary = await GetAsync<Dictionary>(uri, context);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (!string.IsNullOrWhiteSpace(locale))
    {
      Uri uri = new($"{Path}/locale:{locale}", UriKind.Relative);
      Dictionary? dictionary = await GetAsync<Dictionary>(uri, context);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw new TooManyResultsException<Dictionary>(expectedCount: 1, actualCount: dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }

  public async Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<Dictionary>(uri, payload, context);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.IsEmpty.HasValue)
    {
      builder.SetQuery("empty", payload.IsEmpty.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<Dictionary>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<Dictionary>(uri, payload, context);
  }
}
