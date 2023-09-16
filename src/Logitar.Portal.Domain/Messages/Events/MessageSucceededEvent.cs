using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageSucceededEvent : DomainEvent, INotification
{
  public MessageSucceededEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public Dictionary<string, string> Result { get; init; } = new();
}
