using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

internal interface IRealmManager
{
  Task SaveAsync(RealmAggregate realm, ActorId actorId = default, CancellationToken cancellationToken = default);
}
