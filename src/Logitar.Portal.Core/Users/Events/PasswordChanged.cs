using Logitar.EventSourcing;
using Logitar.Portal.Core.Security;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record PasswordChanged : DomainEvent, INotification
{
  public Pbkdf2 Password { get; init; } = null!;
}
