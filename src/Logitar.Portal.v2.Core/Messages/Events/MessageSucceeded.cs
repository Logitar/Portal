using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Events;

public record MessageSucceeded : DomainEvent, INotification
{
  public SendMessageResult Result { get; init; } = new();
}
