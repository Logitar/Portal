using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record DisabledChanged : DomainEvent, INotification
{
  public bool IsDisabled { get; init; }
}
