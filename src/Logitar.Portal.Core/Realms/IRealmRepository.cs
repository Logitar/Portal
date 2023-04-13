using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Templates;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<RealmAggregate> LoadAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task<RealmAggregate> LoadAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task<RealmAggregate> LoadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
