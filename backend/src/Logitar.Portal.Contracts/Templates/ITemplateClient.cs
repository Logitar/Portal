using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Templates;

public interface ITemplateClient
{
  Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, IRequestContext? context = null);
  Task<TemplateModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<TemplateModel?> ReadAsync(Guid? id = null, string? uniqueKey = null, IRequestContext? context = null);
  Task<TemplateModel?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<TemplateModel>> SearchAsync(SearchTemplatesPayload payload, IRequestContext? context = null);
  Task<TemplateModel?> UpdateAsync(Guid id, UpdateTemplatePayload payload, IRequestContext? context = null);
}
