using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events
{
  public record SessionDeletedEvent : DomainEvent, INotification
  {
  }
}
