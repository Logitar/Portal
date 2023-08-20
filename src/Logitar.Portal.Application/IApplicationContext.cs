using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  Actor Actor { get; }
  ActorId ActorId { get; }
}
