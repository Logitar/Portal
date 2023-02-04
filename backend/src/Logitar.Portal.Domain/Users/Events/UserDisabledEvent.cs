using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserDisabledEvent : DomainEvent, INotification
  {
  }
}
