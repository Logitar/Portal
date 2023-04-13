using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Messages.Events;

public record MessageSucceeded : DomainEvent, INotification
{
  public SendMessageResult Result { get; init; } = new();
}
