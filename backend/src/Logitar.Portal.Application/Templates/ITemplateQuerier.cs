using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates;

public interface ITemplateQuerier
{
  Task<Template> ReadAsync(RealmModel? realm, TemplateAggregate template, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(RealmModel? realm, TemplateId id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(RealmModel? realm, string uniqueKey, CancellationToken cancellationToken = default);
  Task<SearchResults<Template>> SearchAsync(RealmModel? realm, SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
}
