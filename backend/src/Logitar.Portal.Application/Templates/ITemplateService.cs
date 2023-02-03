using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Application.Templates
{
  public interface ITemplateService
  {
    Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<TemplateModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<TemplateModel>> GetAsync(string? realm = null, string? search = null,
      TemplateSort? sort = null, bool isDescending = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
    Task<TemplateModel> UpdateAsync(string id, UpdateTemplatePayload payload, CancellationToken cancellationToken = default);
  }
}
