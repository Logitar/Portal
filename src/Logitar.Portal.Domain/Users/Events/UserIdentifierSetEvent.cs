using Logitar.EventSourcing;
using Logitar.Portal.Domain.Events;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserIdentifierSetEvent : IdentifierSetEvent, INotification
{
  public UserIdentifierSetEvent(ActorId actorId) : base(actorId)
  {
  }
}
