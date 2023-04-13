using Logitar.Portal.Core.Security;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Events;

public record SessionCreated : SessionSaved, INotification
{
  public Pbkdf2? Key { get; init; }
}
