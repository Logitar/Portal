using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserEnabledEvent : DomainEvent, INotification
  {
  }
}
