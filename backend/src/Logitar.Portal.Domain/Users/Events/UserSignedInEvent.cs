using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserSignedInEvent : DomainEvent, INotification
  {
  }
}
