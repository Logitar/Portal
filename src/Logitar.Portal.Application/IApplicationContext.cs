using Logitar.EventSourcing;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }
  ConfigurationAggregate Configuration { get; }
}
