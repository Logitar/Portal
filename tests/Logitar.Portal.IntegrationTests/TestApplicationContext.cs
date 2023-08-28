using Logitar.EventSourcing;
using Logitar.Portal.Application;

namespace Logitar.Portal;

internal class TestApplicationContext : IApplicationContext
{
  public ActorId ActorId { get; set; }
}
