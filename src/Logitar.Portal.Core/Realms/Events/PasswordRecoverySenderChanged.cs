using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Realms.Events;

public record PasswordRecoverySenderChanged : DomainEvent, INotification
{
  public AggregateId? SenderId { get; init; }
}
