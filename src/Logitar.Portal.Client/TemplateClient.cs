using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Client;

internal class TemplateClient : ClientBase, ITemplateService
{
  private const string Path = "/api/templates";

  public TemplateClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Template> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Template>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<Template?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<Template>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Template?> ReadAsync(Guid? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Template> templates = new(capacity: 2);

    if (id.HasValue)
    {
      Template? template = await GetAsync<Template>($"{Path}/{id}", cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (uniqueName != null)
    {
      StringBuilder path = new();

      path.Append(Path).Append("/unique-name:").Append(uniqueName);
      if (realm != null)
      {
        path.Append("?realm=").Append(realm);
      }

      Template? template = await GetAsync<Template>(path.ToString(), cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw new TooManyResultsException<Template>(expected: 1, actual: templates.Count);
    }

    return templates.Values.SingleOrDefault();
  }

  public async Task<Template?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<Template>(path.ToString(), payload, cancellationToken);
  }

  public async Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Template>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<Template>($"{Path}/{id}", payload, cancellationToken);
  }
}
