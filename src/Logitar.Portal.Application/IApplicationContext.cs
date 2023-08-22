using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  Actor Actor { get; }
  ActorId ActorId { get; }
  ConfigurationAggregate Configuration { get; }
}
