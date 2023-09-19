using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates;

public interface ITemplateQuerier
{
  Task<Template> ReadAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(string? realm, string uniqueName, CancellationToken cancellationToken = default);
  Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
}
