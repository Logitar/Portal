using Logitar.EventSourcing;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }
  ConfigurationAggregate Configuration { get; set; }
  RealmAggregate? Realm { get; set; }
}
