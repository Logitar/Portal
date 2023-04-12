using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Emails.Templates.Models;
using Logitar.Portal.Core.Emails.Templates.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class TemplateService : HttpService, ITemplateService
  {
    private const string BasePath = "/templates";

    public TemplateService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
      => await PostAsync<TemplateModel>(BasePath, payload, cancellationToken);

    public async Task<TemplateModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
      => await DeleteAsync<TemplateModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<TemplateModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<TemplateModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<TemplateSummary>> GetAsync(string? realm, string? search, TemplateSort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(realm)] = realm,
        [nameof(search)] = search,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<TemplateSummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<TemplateModel> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
      => await PutAsync<TemplateModel>($"{BasePath}/{id}", payload, cancellationToken);
  }
}
