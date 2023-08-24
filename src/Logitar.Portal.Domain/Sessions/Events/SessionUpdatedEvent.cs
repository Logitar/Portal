using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionUpdatedEvent : DomainEvent, INotification
{
  public Dictionary<string, string?> CustomAttributes { get; init; } = new();
}
