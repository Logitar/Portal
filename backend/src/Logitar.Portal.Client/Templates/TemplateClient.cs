using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Client.Templates;

internal class TemplateClient : BaseClient, ITemplateClient
{
  private const string Path = "/api/templates";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public TemplateClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, IRequestContext? context)
  {
    return await PostAsync<TemplateModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<TemplateModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<TemplateModel>(uri, context);
  }

  public async Task<TemplateModel?> ReadAsync(Guid? id, string? uniqueKey, IRequestContext? context)
  {
    Dictionary<Guid, TemplateModel> templates = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      TemplateModel? template = await GetAsync<TemplateModel>(uri, context);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueKey))
    {
      Uri uri = new($"{Path}/unique-key:{uniqueKey}", UriKind.Relative);
      TemplateModel? template = await GetAsync<TemplateModel>(uri, context);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw TooManyResultsException<TemplateModel>.ExpectedSingle(templates.Count);
    }

    return templates.Values.SingleOrDefault();
  }

  public async Task<TemplateModel?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<TemplateModel>(uri, payload, context);
  }

  public async Task<SearchResults<TemplateModel>> SearchAsync(SearchTemplatesPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (!string.IsNullOrWhiteSpace(payload.ContentType))
    {
      builder.SetQuery("type", payload.ContentType);
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<TemplateModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<TemplateModel?> UpdateAsync(Guid id, UpdateTemplatePayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<TemplateModel>(uri, payload, context);
  }
}
