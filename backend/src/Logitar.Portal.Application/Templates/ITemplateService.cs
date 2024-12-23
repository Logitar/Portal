using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Application.Templates;

public interface ITemplateService
{
  Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken = default);
  Task<TemplateModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateModel?> ReadAsync(Guid? id = null, string? uniqueKey = null, CancellationToken cancellationToken = default);
  Task<TemplateModel?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<TemplateModel>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
  Task<TemplateModel?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken = default);
}
