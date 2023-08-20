using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal;

internal class HttpApplicationContext : IApplicationContext
{
  public Actor Actor { get; } = new(); // TODO(fpion): Authentication
  public ActorId ActorId => new(Actor.Id);
}
