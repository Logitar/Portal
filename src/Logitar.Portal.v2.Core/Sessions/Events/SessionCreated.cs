using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Security;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Events;

public record SessionCreated : DomainEvent, INotification
{
  public Pbkdf2? Key { get; init; }

  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
