using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Templates;

public interface ITemplateClient
{
  Task<Template> CreateAsync(CreateTemplatePayload payload, IRequestContext? context = null);
  Task<Template?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<Template?> ReadAsync(Guid? id = null, string? uniqueKey = null, IRequestContext? context = null);
  Task<Template?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, IRequestContext? context = null);
  Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, IRequestContext? context = null);
}
