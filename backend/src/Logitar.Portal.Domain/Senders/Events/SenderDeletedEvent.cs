using MediatR;

namespace Logitar.Portal.Domain.Senders.Events
{
  public record SenderDeletedEvent : DomainEvent, INotification
  {
  }
}
