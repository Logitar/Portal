using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Emails.Templates.Models;
using Logitar.Portal.Core.Emails.Templates.Payloads;

namespace Logitar.Portal.Client
{
  public interface ITemplateService
  {
    Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken = default);
    Task<TemplateModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TemplateModel> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<TemplateSummary>> GetAsync(string? realm = null, string? search = null,
      TemplateSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<TemplateModel> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken = default);
  }
}
