namespace Logitar.Portal.Contracts.Templates;

public interface ITemplateService
{
  Task<Template> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken = default);
  Task<Template?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(Guid? id = null, string? realm = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<Template?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
  Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken = default);
}
