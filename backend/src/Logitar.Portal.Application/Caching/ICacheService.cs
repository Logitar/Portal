using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  Configuration? Configuration { get; set; }

  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(Actor actor);
}
