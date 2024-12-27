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

  public async Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, IRequestContext? context)
  {
    return await PostAsync<DictionaryModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<DictionaryModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<DictionaryModel>(uri, context);
  }

  public async Task<DictionaryModel?> ReadAsync(Guid? id, string? locale, IRequestContext? context)
  {
    Dictionary<Guid, DictionaryModel> dictionaries = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      DictionaryModel? dictionary = await GetAsync<DictionaryModel>(uri, context);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (!string.IsNullOrWhiteSpace(locale))
    {
      Uri uri = new($"{Path}/locale:{locale}", UriKind.Relative);
      DictionaryModel? dictionary = await GetAsync<DictionaryModel>(uri, context);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw TooManyResultsException<DictionaryModel>.ExpectedSingle(dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }

  public async Task<DictionaryModel?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<DictionaryModel>(uri, payload, context);
  }

  public async Task<SearchResults<DictionaryModel>> SearchAsync(SearchDictionariesPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.IsEmpty.HasValue)
    {
      builder.SetQuery("empty", payload.IsEmpty.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<DictionaryModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<DictionaryModel?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<DictionaryModel>(uri, payload, context);
  }
}
