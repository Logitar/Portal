using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  ConfigurationAggregate? Configuration { get; set; }

  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(ActorId id, Actor actor);

  Session? GetSession(Guid id);
  void RemoveSession(SessionAggregate session);
  void SetSession(Session session);

  CachedUser? GetUser(string uniqueName);
  void RemoveUser(UserAggregate user);
  void SetUser(CachedUser user);
}
