using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Templates;

public interface ITemplateRepository
{
  Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(AggregateId id, long? version = null, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken = default);
  Task<IEnumerable<TemplateAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken = default);
}
