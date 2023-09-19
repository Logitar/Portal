using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageFailedEvent : DomainEvent, INotification
{
  public MessageFailedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public Dictionary<string, string> Result { get; init; } = new();
}
