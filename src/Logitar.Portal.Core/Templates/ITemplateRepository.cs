using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Templates;

public interface ITemplateRepository
{
  Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<TemplateAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadByUniqueNameAsync(RealmAggregate realm, string uniqueName, CancellationToken cancellationToken = default);
  Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken = default);
}
