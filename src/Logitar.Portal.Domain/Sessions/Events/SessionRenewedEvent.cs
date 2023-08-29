using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionRenewedEvent : DomainEvent, INotification
{
  public SessionRenewedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public Password? Secret { get; init; }
}
