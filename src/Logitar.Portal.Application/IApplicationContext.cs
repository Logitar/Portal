using Logitar.EventSourcing;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }
}
