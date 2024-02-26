using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageSucceededEvent : DomainEvent, INotification
{
  public IReadOnlyDictionary<string, string> ResultData { get; }

  public MessageSucceededEvent(ActorId actorId, IReadOnlyDictionary<string, string> resultData)
  {
    ActorId = actorId;
    ResultData = resultData;
  }
}
