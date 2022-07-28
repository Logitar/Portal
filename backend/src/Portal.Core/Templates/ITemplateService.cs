using Portal.Core.Templates.Models;
using Portal.Core.Templates.Payloads;

namespace Portal.Core.Templates
{
  public interface ITemplateService
  {
    Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken = default);
    Task<TemplateModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TemplateModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<TemplateModel>> GetAsync(Guid? realmId = null, string? search = null,
      TemplateSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<TemplateModel> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken = default);
  }
}
