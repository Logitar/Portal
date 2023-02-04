using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserDeletedEvent : DomainEvent, INotification
  {
  }
}
