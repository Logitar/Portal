using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Templates;

public interface ITemplateRepository
{
  Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Template>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Template>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(TenantId? tenantId, Identifier uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(Template template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken = default);
}
