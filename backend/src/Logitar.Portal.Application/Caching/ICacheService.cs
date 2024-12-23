using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  ConfigurationModel? Configuration { get; set; }

  ActorModel? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(ActorModel actor);
}
