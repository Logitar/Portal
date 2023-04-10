using Logitar.Portal.v2.Core.Security;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Events;

public record SessionRefreshed : SessionSaved, INotification
{
  public Pbkdf2 Key { get; init; } = null!;
}
