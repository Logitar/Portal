using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }
  Realm Realm { get; set; }
}
