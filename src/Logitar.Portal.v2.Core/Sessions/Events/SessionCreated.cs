using Logitar.Portal.v2.Core.Security;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Events;

public record SessionCreated : SessionSaved, INotification
{
  public Pbkdf2? Key { get; init; }
}
