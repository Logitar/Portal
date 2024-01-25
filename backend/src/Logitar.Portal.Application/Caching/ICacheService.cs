using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void SetActor(Actor actor);
  void RemoveActor(ActorId id);

  Configuration? GetConfiguration();
  void SetConfiguration(Configuration configuration);
}
