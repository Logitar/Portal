using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Templates;

public interface ITemplateRepository
{
  Task<TemplateAggregate?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<TemplateAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<TemplateAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(TenantId? tenantId, Identifier uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken = default);
}
