using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events
{
  public record SessionSignedOutEvent : DomainEvent, INotification
  {
  }
}
