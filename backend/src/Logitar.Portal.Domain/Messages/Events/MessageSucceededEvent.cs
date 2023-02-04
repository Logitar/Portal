using MediatR;

namespace Logitar.Portal.Domain.Messages.Events
{
  public record MessageSucceededEvent : DomainEvent, INotification
  {
    public SendMessageResult Result { get; init; } = new();
  }
}
