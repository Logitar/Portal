using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Caching;

public interface ICacheService
{
  RealmAggregate? PortalRealm { get; set; }

  Actor? GetActor(Guid id);
  void RemoveActor(Guid id);
  void SetActor(Actor actor);

  Session? GetSession(Guid id);
  void RemoveSession(SessionAggregate session);
  void SetSession(Session session);

  CachedUser? GetUser(string username);
  void RemoveUser(UserAggregate user);
  void SetUser(string username, CachedUser user);
}
