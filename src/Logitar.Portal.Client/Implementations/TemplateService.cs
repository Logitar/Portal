using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Client.Implementations;

internal class TemplateService : HttpService, ITemplateService
{
  private const string BasePath = "/templates";

  public TemplateService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Template> CreateAsync(CreateTemplateInput input, CancellationToken cancellationToken)
    => await PostAsync<Template>(BasePath, input, cancellationToken);

  public async Task<Template> DeleteAsync(Guid id, CancellationToken cancellationToken)
    => await DeleteAsync<Template>($"{BasePath}/{id}", cancellationToken);

  public async Task<Template?> GetAsync(Guid? id, string? realm, string? key, CancellationToken cancellationToken)
  {
    if (realm != null && key != null)
    {
      throw new NotSupportedException("You may only query templates by their identifier.");
    }

    return id.HasValue ? await GetAsync<Template>($"{BasePath}/{id.Value}", cancellationToken) : null;
  }

  public async Task<PagedList<Template>> GetAsync(string? realm, string? search,
    TemplateSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(realm)] = realm,
      [nameof(search)] = search,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<Template>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<Template> UpdateAsync(Guid id, UpdateTemplateInput input, CancellationToken cancellationToken)
    => await PutAsync<Template>($"{BasePath}/{id}", input, cancellationToken);
}
