using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Security;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Events;

public record PasswordChanged : DomainEvent, INotification
{
  public Pbkdf2 Password { get; init; } = null!;
}
