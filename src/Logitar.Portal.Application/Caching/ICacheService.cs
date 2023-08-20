using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(ActorId id, Actor actor);
}
