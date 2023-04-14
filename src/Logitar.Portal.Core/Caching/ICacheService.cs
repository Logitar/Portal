using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Caching;

public interface ICacheService
{
  RealmAggregate? PortalRealm { get; set; }

  Actor? GetActor(Guid id);
  void RemoveActor(Guid id);
  void SetActor(Actor actor);
}
