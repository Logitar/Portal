using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Templates;

public interface ITemplateRepository
{
  Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken = default);
  Task<IEnumerable<TemplateAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<TemplateAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(TenantId? tenantId, UniqueKey uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken = default);
}
