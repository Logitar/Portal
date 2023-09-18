using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserPasswordResetEvent : DomainEvent, INotification
{
  public UserPasswordResetEvent(ActorId actorId, Password password)
  {
    ActorId = actorId;
    Password = password;
  }

  public Password Password { get; }
}
