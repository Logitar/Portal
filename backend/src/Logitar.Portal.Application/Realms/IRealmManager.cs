using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

internal interface IRealmManager
{
  Task SaveAsync(Realm realm, ActorId actorId = default, CancellationToken cancellationToken = default);
}
