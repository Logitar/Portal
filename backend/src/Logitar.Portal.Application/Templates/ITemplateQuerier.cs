using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates;

public interface ITemplateQuerier
{
  Task<Template> ReadAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(TemplateId id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(string uniqueKey, CancellationToken cancellationToken = default);
  Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
}
