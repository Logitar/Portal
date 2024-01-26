using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void SetActor(Actor actor);
  void RemoveActor(ActorId id);

  ConfigurationAggregate? GetConfiguration();
  void RemoveConfiguration();
  void SetConfiguration(ConfigurationAggregate configuration);
}
