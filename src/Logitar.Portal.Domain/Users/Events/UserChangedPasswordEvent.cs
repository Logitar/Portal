using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserChangedPasswordEvent : DomainEvent, INotification
{
  public UserChangedPasswordEvent(ActorId actorId, Password newPassword)
  {
    ActorId = actorId;
    NewPassword = newPassword;
  }

  public Password NewPassword { get; }
}
