using Logitar.EventSourcing;
using Logitar.Portal.Application;

namespace Logitar.Portal.Web;

internal class HttpApplicationContext : IApplicationContext
{
  public ActorId ActorId { get; } = new();
}
