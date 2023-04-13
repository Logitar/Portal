using Logitar.Portal.Core.Security;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Events;

public record SessionRefreshed : SessionSaved, INotification
{
  public Pbkdf2 Key { get; init; } = null!;
}
